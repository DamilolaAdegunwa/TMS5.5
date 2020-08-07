using Egoal.Application.Services;
using Egoal.Application.Services.Dto;
using Egoal.Authorization;
using Egoal.AutoMapper;
using Egoal.Caches;
using Egoal.Common;
using Egoal.Cryptography;
using Egoal.Customers;
using Egoal.Domain.Repositories;
using Egoal.Domain.Uow;
using Egoal.DynamicCodes;
using Egoal.Extensions;
using Egoal.Members.Dto;
using Egoal.Orders;
using Egoal.Payment;
using Egoal.Runtime.Security;
using Egoal.Runtime.Session;
using Egoal.Scenics;
using Egoal.Staffs;
using Egoal.Tickets;
using Egoal.Tickets.Dto;
using Egoal.TicketTypes;
using Egoal.Trades;
using Egoal.UI;
using Egoal.WeChat.OAuth;
using Egoal.WeChat.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Egoal.Members
{
    public class MemberAppService : ApplicationService, IMemberAppService
    {
        private readonly IHostingEnvironment _environment;
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;
        private readonly ISession _session;
        private readonly IDynamicCodeService _dynamicCodeService;
        private readonly IServiceProvider _serviceProvider;
        private readonly INameCacheService _nameCacheService;
        private readonly IRepository<Member, Guid> _memberRepository;
        private readonly IRepository<MemberCard> _memberCardRepository;
        private readonly IRepository<UserWechat> _userRepository;
        private readonly IRepository<CustomerMemberBind> _customerMemberBindRepository;
        private readonly IRepository<MemberPhoto, Guid> _memberPhotoRepository;
        private readonly IRepository<Order, string> _orderRepository;
        private readonly IRepository<OrderDetail, long> _orderDetailRepository;
        private readonly IRepository<TicketCheck, long> _ticketCheckRepository;
        private readonly IRepository<TicketGround, long> _ticketGroundRepository;
        private readonly IRepository<TicketGroundCache, long> _ticketGroundCacheRepository;
        private readonly IRepository<TicketSale, long> _ticketSaleRepository;
        private readonly IRepository<Trade, Guid> _tradeRepository;
        private readonly ISignInAppService _signInAppService;
        private readonly ITicketSaleDomainService _ticketSaleDomainService;
        private readonly IMemberDomainService _memberDomainService;
        private readonly IStaffDomainService _staffDomainService;
        private readonly ICustomerDomainService _customerDomainService;
        private readonly IRightDomainService _rightDomainService;
        private readonly ICommonAppService _commonAppService;

        public MemberAppService(
            IHostingEnvironment environment,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider,
            ISession session,
            IDynamicCodeService dynamicCodeService,
            IServiceProvider serviceProvider,
            INameCacheService nameCacheService,
            IRepository<Member, Guid> memberRepository,
            IRepository<MemberCard> memberCardRepository,
            IRepository<UserWechat> userRepository,
            IRepository<CustomerMemberBind> customerMemberBindRepository,
            IRepository<MemberPhoto, Guid> memberPhotoRepository,
            IRepository<Order, string> orderRepository,
            IRepository<OrderDetail, long> orderDetailRepository,
            IRepository<TicketCheck, long> ticketCheckRepository,
            IRepository<TicketGround, long> ticketGroundRepository,
            IRepository<TicketGroundCache, long> ticketGroundCacheRepository,
            IRepository<TicketSale, long> ticketSaleRepository,
            IRepository<Trade, Guid> tradeRepository,
            ISignInAppService signInAppService,
            ITicketSaleDomainService ticketSaleDomainService,
            IMemberDomainService memberDomainService,
            IStaffDomainService staffDomainService,
            ICustomerDomainService customerDomainService,
            IRightDomainService rightDomainService,
            ICommonAppService commonAppService)
        {
            _environment = environment;
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
            _session = session;
            _dynamicCodeService = dynamicCodeService;
            _serviceProvider = serviceProvider;
            _nameCacheService = nameCacheService;
            _memberRepository = memberRepository;
            _memberCardRepository = memberCardRepository;
            _customerMemberBindRepository = customerMemberBindRepository;
            _memberPhotoRepository = memberPhotoRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _ticketCheckRepository = ticketCheckRepository;
            _ticketGroundRepository = ticketGroundRepository;
            _ticketGroundCacheRepository = ticketGroundCacheRepository;
            _ticketSaleRepository = ticketSaleRepository;
            _tradeRepository = tradeRepository;
            _userRepository = userRepository;
            _signInAppService = signInAppService;
            _ticketSaleDomainService = ticketSaleDomainService;
            _memberDomainService = memberDomainService;
            _staffDomainService = staffDomainService;
            _customerDomainService = customerDomainService;
            _rightDomainService = rightDomainService;
            _commonAppService = commonAppService;
        }

        public async Task<LoginOutput> LoginFromWechatOffiaccountAsync(WechatOffiaccountLoginInput input)
        {
            Member member = null;
            UserWechat user = null;

            if (_environment.IsProduction())
            {
                if (input.Code.IsNullOrEmpty())
                {
                    throw new ArgumentNullException("Code不能为空");
                }

                var oAuthApi = _serviceProvider.GetRequiredService<OAuthApi>();
                var result = await oAuthApi.GetUserAccessTokenAsync(input.Code);

                var userApi = _serviceProvider.GetRequiredService<UserApi>();
                var wxUser = await userApi.GetUserInfoAsync(result.openid);

                user = await _userRepository.GetAll()
                    .WhereIf(wxUser.unionid.IsNullOrEmpty(), u => u.OffiaccountOpenId == result.openid)
                    .WhereIf(!wxUser.unionid.IsNullOrEmpty(), u => u.OffiaccountOpenId == result.openid || u.UnionId == wxUser.unionid)
                    .FirstOrDefaultAsync();
                if (user == null)
                {
                    user = new UserWechat();
                    user.Nickname = wxUser.nickname;
                    user.HeadImageUrl = wxUser.headimgurl;

                    member = await _memberRepository.FirstOrDefaultAsync(o => o.Uid == result.openid);
                    if (member == null)
                    {
                        member = await CreateWeChatMemberAsync(wxUser, user);
                    }
                    else
                    {
                        member.UserWechat = user;
                    }
                }
                else
                {
                    member = await _memberRepository.FirstOrDefaultAsync(user.UserId);
                }

                user.OffiaccountOpenId = result.openid;
                user.UnionId = wxUser.unionid;
                member.IsWeChatSubscribed = wxUser.subscribe == "1";
            }
            else
            {
                //member = await _memberRepository.FirstOrDefaultAsync(Guid.Parse("F07C3AFC-64E4-48EF-8179-674777C7D80F"));
                member = await _memberRepository.FirstOrDefaultAsync(Guid.Parse("90EEAE6C-AE10-4833-874B-E40ABB0F677A"));
                user = await _userRepository.FirstOrDefaultAsync(u => u.UserId == member.Id);
            }

            var output = await BuildLoginOutputAsync(member, user.OffiaccountOpenId);

            return output;
        }


        public async Task<LoginOutput> LoginFromWechatMiniProgramAsync(WechatMiniProgramLoginInput input)
        {
            if (input.Code.IsNullOrEmpty())
            {
                throw new ArgumentNullException("Code不能为空");
            }

            var oAuthApi = _serviceProvider.GetRequiredService<OAuthApi>();
            var result = await oAuthApi.Code2SessionAsync(input.Code);

            Member member = null;

            var user = await _userRepository.GetAll()
                .WhereIf(result.unionid.IsNullOrEmpty(), u => u.MiniProgramOpenId == result.openid)
                .WhereIf(!result.unionid.IsNullOrEmpty(), u => u.MiniProgramOpenId == result.openid || u.UnionId == result.unionid)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                user = new UserWechat();
                user.HeadImageUrl = input.UserInfo.AvatarUrl;
                user.Nickname = input.UserInfo.NickName;

                member = await CreateWeChatMemberAsync(null, user);
            }
            else
            {
                member = await _memberRepository.FirstOrDefaultAsync(user.UserId);
            }

            user.MiniProgramOpenId = result.openid;
            user.UnionId = result.unionid;

            var output = await BuildLoginOutputAsync(member, result.openid);

            return output;
        }

        private async Task<Member> CreateWeChatMemberAsync(WeChat.User.UserInfoResult wxUser, UserWechat user)
        {
            var member = new Member();
            if (wxUser != null)
            {
                member.Name = member.PetName = wxUser.nickname;
                member.HeadImgUrl = wxUser.headimgurl;
                if (wxUser.sex == "1")
                {
                    member.Sex = "男";
                }
                else if (wxUser.sex == "2")
                {
                    member.Sex = "女";
                }
                member.Address = wxUser.Address;
            }
            else
            {
                member.Name = user.Nickname;
                member.HeadImgUrl = user.HeadImageUrl;
            }
            member.RegTypeId = RegType.会员;
            member.Uid = DateTime.Now.Ticks.ToString();
            member.Salt = DateTime.Now.AddMinutes(30).Ticks.ToString();
            member.Pwd = SHAHelper.SHA512Encrypt("000000", member.Salt);
            member.Code = DateTime.Now.AddHours(1).Ticks.ToString();
            member.MemberTypeId = (int)MemberType.微信会员;
            member.MemberTypeName = MemberType.微信会员.ToString();
            member.MemberStatusId = MemberStatus.正常;
            member.MemberStatusName = MemberStatus.正常.ToString();
            member.SourceType = ((int)SourceType.微信).ToString();

            member.UserWechat = user;

            return await _memberRepository.InsertAsync(member);
        }

        public async Task<LoginOutput> BindStaffAsync(BindStaffInput input)
        {
            var staff = await _staffDomainService.LoginAsync(input.UserName, input.Password);

            var member = await _memberRepository.GetAsync(_session.MemberId.Value);
            member.BindStaff(staff.Id);

            var user = await _userRepository.FirstOrDefaultAsync(u => u.UserId == member.Id);

            var output = await BuildLoginOutputAsync(member, user.OffiaccountOpenId);

            return output;
        }

        public async Task<LoginOutput> BuildLoginOutputAsync(Member member, string openId, Guid? customerId = null)
        {
            var loginOutput = new LoginOutput();
            loginOutput.Member = member.MapTo<MemberDto>();
            loginOutput.Member.OpenId = openId;
            loginOutput.Member.LocalTicketEnrollFace = await _rightDomainService.HasFeatureAsync(Guid.Parse(Permissions.TMSWeChat_EnrollFace));

            var bindingCustomerId = await _customerDomainService.GetBindingCustomerIdAsync(member.Id);
            loginOutput.Member.HasBindCustomer = bindingCustomerId.HasValue;
            customerId = customerId ?? bindingCustomerId;
            if (customerId.HasValue)
            {
                loginOutput.Member.CustomerId = customerId;
                loginOutput.Member.CustomerName = _nameCacheService.GetCustomerName(customerId);
            }

            if (member.StaffId.HasValue)
            {
                var permissions = await _staffDomainService.GetPermissionsAsync(member.StaffId.Value, SystemType.微信购票系统);
                loginOutput.Permissions.AddRange(permissions);
            }

            if (member.SourceType != ((int)SourceType.售票).ToString())
            {
                loginOutput.Member.IsRegisted = false;
            }
            else
            {
                loginOutput.Member.IsRegisted = true;
            }

            var claims = await BuildMemberClaimsAsync(member, customerId);
            loginOutput.Token = _signInAppService.CreateToken(claims);

            return loginOutput;
        }

        private async Task<List<Claim>> BuildMemberClaimsAsync(Member member, Guid? customerId)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(TmsClaimTypes.MemberId, member.Id.ToString()));

            if (member.StaffId.HasValue)
            {
                claims.Add(new Claim(TmsClaimTypes.StaffId, member.StaffId.Value.ToString()));
                var roleId = await _staffDomainService.GetRoleIdAsync(member.StaffId.Value);
                if (roleId.HasValue)
                {
                    claims.Add(new Claim(TmsClaimTypes.RoleId, roleId.Value.ToString()));
                }
            }
            else
            {
                claims.Add(new Claim(TmsClaimTypes.StaffId, DefaultStaff.微信购票.ToString()));
            }

            if (customerId.HasValue)
            {
                claims.Add(new Claim(TmsClaimTypes.CustomerId, customerId.Value.ToString()));
            }

            claims.Add(new Claim(TmsClaimTypes.PcId, DefaultPc.微信购票.ToString()));
            claims.Add(new Claim(TmsClaimTypes.SalePointId, DefaultSalePoint.微信购票.ToString()));
            claims.Add(new Claim(TmsClaimTypes.ParkId, DefaultPark.微信购票.ToString()));

            return claims;
        }

        public async Task RegistWechatMemberAsync(RegistWechatMemberInput input)
        {
            var member = await _memberRepository.FirstOrDefaultAsync(_session.MemberId.Value);
            member.Name = input.Name;
            member.Mobile = input.Mobile;
            member.SourceType = ((int)SourceType.售票).ToString();
        }

        public async Task RegistMemberCardAsync(RegistMemberCardInput input, Guid memberId)
        {
            if (await _memberDomainService.HasElectronicMemberCardAsync(memberId))
            {
                return;
            }

            var member = await _memberRepository.GetAsync(memberId);
            member.Name = input.Name;
            member.Mobile = input.Mobile;
            member.CertNo = input.IdCard;
            member.CertTypeId = DefaultCertType.二代身份证;
            member.CertTypeName = "二代身份证";
            member.Sex = input.Sex;
            member.Nation = input.Nation;
            member.Education = input.Education;
            member.Address = input.Address;

            SaleTicketInput saleTicketInput = new SaleTicketInput();
            saleTicketInput.TravelDate = DateTime.Now.Date;
            saleTicketInput.ListNo = await _dynamicCodeService.GenerateListNoAsync(ListNoType.门票网上订票);
            saleTicketInput.TradeTypeTypeId = TradeTypeType.会员卡;
            saleTicketInput.TradeTypeId = DefaultTradeType.会员卡销售;
            saleTicketInput.TradeTypeName = "会员卡销售";
            saleTicketInput.TradeSource = TradeSource.微信;
            saleTicketInput.PayTypeId = DefaultPayType.现金;
            saleTicketInput.PayTypeName = "现金";
            saleTicketInput.PayFlag = true;
            saleTicketInput.MemberId = member.Id;
            saleTicketInput.MemberName = member.Name;
            saleTicketInput.CashierId = _session.StaffId;
            saleTicketInput.CashPcid = _session.PcId;
            saleTicketInput.SalePointId = _session.SalePointId;
            saleTicketInput.ParkId = _session.ParkId;

            saleTicketInput.Items.Add(new SaleTicketItem { TicketTypeId = DefaultTicketType.电子会员卡, Quantity = 1 });

            var createTicketAppService = _serviceProvider.GetRequiredService<CreateTicketAppService>();
            var ticketSales = await createTicketAppService.SaleAsync(saleTicketInput);

            await _currentUnitOfWorkProvider.Current.SaveChangesAsync();

            foreach (var ticketSale in ticketSales)
            {
                var memberCard = ticketSale.MapToMemberCard();
                memberCard.IsElectronicTicket = true;
                memberCard.AviateFlag = ticketSale.TicketType != null && ticketSale.TicketType.FirstActiveFlag != true;

                await _memberCardRepository.InsertAsync(memberCard);
            }
        }

        public async Task<MemberCardDto> GetElectronicMemberCardAsync(Guid memberId)
        {
            var memberCard = await _memberCardRepository.FirstOrDefaultAsync(m => m.MemberId == memberId && m.TicketTypeId == DefaultTicketType.电子会员卡);

            var memberCardDto = new MemberCardDto();
            memberCardDto.Id = memberCard.Id;
            memberCardDto.TicketTypeName = memberCard.TicketTypeName;
            memberCardDto.Etime = memberCard.Etime;
            memberCardDto.TicketCode = memberCard.TicketCode;
            memberCardDto.IsExpired = memberCard.Etime.To<DateTime>() < DateTime.Now.Date;
            if (memberCardDto.IsExpired)
            {
                var ticketTypeRepository = _serviceProvider.GetRequiredService<ITicketTypeRepository>();
                var ticketType = await ticketTypeRepository.GetAsync(memberCard.TicketTypeId.Value);

                memberCardDto.Days = ticketType.DelayDays ?? 0;
            }

            var member = await _memberRepository.GetAsync(memberId);

            memberCardDto.MemberName = member.Name;
            memberCardDto.Mobile = member.Mobile;
            memberCardDto.IdCard = member.CertNo;
            memberCardDto.Sex = member.Sex;
            memberCardDto.Nation = member.Nation;
            memberCardDto.Education = member.Education;
            memberCardDto.Address = member.Address;

            return memberCardDto;
        }

        public async Task RenewMemberCardAsync(int id)
        {
            var memberCard = await _memberCardRepository.FirstOrDefaultAsync(m => m.Id == id);
            if (memberCard == null)
            {
                throw new UserFriendlyException("会员卡不存在");
            }

            var ticketSale = await _ticketSaleDomainService.RenewAsync(memberCard.TicketId.Value);

            memberCard.Renew(ticketSale.Etime.Substring(0, 10), (int)ticketSale.TicketStatusId, ticketSale.TicketStatusName);
        }

        public async Task<List<ComboboxItemDto<Guid>>> GetMemberComboboxItemsAsync()
        {
            var sourceType = ((int)SourceType.售票).ToString();

            var query = _memberRepository.GetAll()
                .Where(m => m.RegTypeId == RegType.会员 && m.SourceType == sourceType)
                .Select(m => new ComboboxItemDto<Guid> { DisplayText = m.Name, Value = m.Id });

            var items = await _memberRepository.ToListAsync(query);

            return items;
        }



        public async Task<string> BindSendVerificationCodeAsync(string mobile)
        {
            string error = "";
            bool ifAny = await _memberRepository.GetAll().AnyAsync(m => m.Mobile == mobile && m.SourceType == ((int)SourceType.售票).ToString());
            if (!ifAny)
            {
                error = "手机号未办理会员";
            }
            else
            {
                ifAny = await _memberRepository.GetAll().AnyAsync(m => m.Mobile == mobile && m.SourceType == ((int)SourceType.微信).ToString());
                if (ifAny)
                {
                    error = "手机号已绑定其它微信号";
                }
                else
                {
                    await _commonAppService.SendVerificationCodeAsync(mobile);
                }
            }
            return error;
        }

        public async Task<string> RegistSendVerificationCodeAsync(string mobile)
        {
            string error = "";
            bool ifAny = await _memberRepository.GetAll().AnyAsync(m => m.Mobile == mobile && m.SourceType == ((int)SourceType.售票).ToString());
            if (ifAny)
            {
                error = "手机号已办理会员，请返回上一级选择绑定";
            }
            else
            {
                ifAny = await _memberRepository.GetAll().AnyAsync(m => m.Mobile == mobile && m.SourceType == ((int)SourceType.微信).ToString());
                if (ifAny)
                {
                    error = "手机号已注册其它微信号";
                }
                else
                {
                    await _commonAppService.SendVerificationCodeAsync(mobile);
                }
            }
            return error;
        }

        public async Task<LoginOutput> BindFromWeChatAsync(BindFromWeChatInput input)
        {
            LoginOutput loginOutput = new LoginOutput();
            var saleMember = await _memberRepository.GetAll().FirstOrDefaultAsync(m => m.Mobile == input.Mobile && m.SourceType == ((int)SourceType.售票).ToString());
            if (saleMember == null)
            {
                loginOutput.Error = "手机号未办理会员";
            }
            else
            {
                var member = await _memberRepository.GetAll().FirstOrDefaultAsync(m => m.Mobile == input.Mobile && m.SourceType == ((int)SourceType.微信).ToString());
                if (member != null)
                {
                    member = await _memberRepository.GetAll().FirstOrDefaultAsync(m => m.Mobile == input.Mobile && m.SourceType == ((int)SourceType.微信).ToString() && m.Id == _session.MemberId);
                    if (member == null)
                    {
                        loginOutput.Error = "手机号已绑定其它微信号";
                    }
                    else
                    {
                        loginOutput = await BuildLoginOutputAsync(member);
                    }
                }
                else
                {
                    member = await _memberRepository.GetAll().FirstOrDefaultAsync(m => m.Id == _session.MemberId);
                    if (member == null)
                    {
                        loginOutput.Error = "获取用户信息失败，请重登购票页面";
                    }
                    else
                    {
                        await ChangeMemberIdAsync(member.Id, saleMember.Id);
                        string openId = member.Uid;
                        await _memberRepository.DeleteAsync(member.Id);
                        saleMember.Uid = openId;
                        loginOutput = await BuildLoginOutputAsync(saleMember);
                    }
                }
            }
            return loginOutput;
        }

        /// <summary>
        /// 更改原会员下的所有记录，到线下已注册会员的MemberId下
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="newMemberId"></param>
        /// <returns></returns>
        private async Task ChangeMemberIdAsync(Guid memberId, Guid newMemberId)
        {
            List<MemberCard> memberCards = await _memberCardRepository.GetAll().Where(t => t.MemberId == memberId).ToListAsync();
            foreach (var item in memberCards)
            {
                item.MemberId = newMemberId;
            }

            List<CustomerMemberBind> customerMemberBinds = await _customerMemberBindRepository.GetAll().Where(t => t.MemberId == memberId).ToListAsync();
            foreach (var item in customerMemberBinds)
            {
                item.MemberId = newMemberId;
            }

            List<MemberPhoto> memberPhotos = await _memberPhotoRepository.GetAll().Where(t => t.MemberId == memberId).ToListAsync();
            foreach (var item in memberPhotos)
            {
                item.MemberId = newMemberId;
            }

            List<Order> orders = await _orderRepository.GetAll().Where(t => t.MemberId == memberId).ToListAsync();
            foreach (var item in orders)
            {
                item.MemberId = newMemberId;
            }

            List<OrderDetail> orderDetails = await _orderDetailRepository.GetAll().Where(t => t.MemberId == memberId).ToListAsync();
            foreach (var item in orderDetails)
            {
                item.MemberId = newMemberId;
            }

            List<TicketCheck> ticketChecks = await _ticketCheckRepository.GetAll().Where(t => t.MemberId == memberId).ToListAsync();
            foreach (var item in ticketChecks)
            {
                item.MemberId = newMemberId;
            }

            List<TicketGround> ticketGrounds = await _ticketGroundRepository.GetAll().Where(t => t.MemberId == memberId).ToListAsync();
            foreach (var item in ticketGrounds)
            {
                item.MemberId = newMemberId;
            }

            List<TicketGroundCache> ticketGroundCaches = await _ticketGroundCacheRepository.GetAll().Where(t => t.MemberId == memberId).ToListAsync();
            foreach (var item in ticketGroundCaches)
            {
                item.MemberId = newMemberId;
            }

            List<TicketSale> ticketSales = await _ticketSaleRepository.GetAll().Where(t => t.MemberId == memberId).ToListAsync();
            foreach (var item in ticketSales)
            {
                item.MemberId = newMemberId;
            }

            List<Trade> trades = await _tradeRepository.GetAll().Where(t => t.MemberId == memberId).ToListAsync();
            foreach (var item in trades)
            {
                item.MemberId = newMemberId;
            }
        }

        public async Task<LoginOutput> RegistFromWeChatAsync(RegistMemberInput input)
        {
            LoginOutput loginOutput = new LoginOutput();
            var member = await _memberRepository.GetAll().FirstOrDefaultAsync(m => m.Mobile == input.Mobile && m.SourceType == ((int)SourceType.售票).ToString());
            if (member != null)
            {
                loginOutput.Error = "手机号已办理会员，请返回上一级选择绑定";
            }
            else
            {
                member = await _memberRepository.GetAll().FirstOrDefaultAsync(m => m.Mobile == input.Mobile && m.SourceType == ((int)SourceType.微信).ToString());
                if (member != null)
                {
                    member = await _memberRepository.GetAll().FirstOrDefaultAsync(m => m.Mobile == input.Mobile && m.SourceType == ((int)SourceType.微信).ToString() && m.Id == _session.MemberId);
                    if (member == null)
                    {
                        loginOutput.Error = "手机号已注册其它微信号";
                    }
                    else
                    {
                        loginOutput = await BuildLoginOutputAsync(member);
                    }
                }
                else
                {
                    member = await _memberRepository.GetAll().FirstOrDefaultAsync(m => m.Id == _session.MemberId);
                    if (member == null)
                    {
                        loginOutput.Error = "获取用户信息失败，请重登购票页面";
                    }
                    else
                    {
                        member.Mobile = input.Mobile;
                        member.SourceType = ((int)SourceType.售票).ToString();
                        loginOutput = await BuildLoginOutputAsync(member);
                    }
                }
            }
            return loginOutput;
        }

        public async Task<PagedResultDto<MemberTicketSaleDto>> GetVipTicketsForMobileAsync(GetMemberTicketsInput input)
        {
            var now = DateTime.Now;

            var query = _ticketSaleRepository.GetAll()
                .Where(t =>
                t.TicketStatusId != TicketStatus.作废 &&
                Convert.ToDateTime(t.Etime) >= now &&
                t.SurplusNum > 0 &&
                t.TicketTypeTypeId == TicketTypeType.会员卡);

            return await GetTicketsForMobileAsync(input, query);
        }

        private async Task<PagedResultDto<MemberTicketSaleDto>> GetTicketsForMobileAsync(GetMemberTicketsInput input, IQueryable<TicketSale> query)
        {
            if (input.CustomerId.HasValue)
            {
                query = query.Where(t => (t.MemberId == input.MemberId && t.CustomerId == null) || t.CustomerId == input.CustomerId.Value);
            }
            else
            {
                query = query.Where(t => t.MemberId == input.MemberId && t.CustomerId == null);
            }

            var count = await _ticketSaleRepository.CountAsync(query);

            query = query.OrderByDescending(t => t.Ctime).PageBy(input);

            var ticketQuery = from ticket in query
                              select new
                              {
                                  ticket.TicketCode,
                                  ticket.TicketTypeName,
                                  StatusName = ticket.TicketStatusName,
                                  StartDate = ticket.Stime,
                                  EndDate = ticket.Etime,
                                  CTime = ticket.Ctime
                              };
            var list = await _ticketSaleRepository.ToListAsync(ticketQuery);

            var tickets = list.Select(t => new MemberTicketSaleDto
            {
                TicketCode = t.TicketCode,
                TicketTypeName = t.TicketTypeName,
                StatusName = t.StatusName,
                StartDate = t.StartDate.Substring(0, 10),
                EndDate = t.EndDate.Substring(0, 10),
                CTime = t.CTime.Value.ToDateTimeString()
            }).ToList();

            return new PagedResultDto<MemberTicketSaleDto>(count, tickets);
        }

        public async Task<LoginOutput> BuildLoginOutputAsync(Member member, Guid? customerId = null)
        {
            var loginOutput = new LoginOutput();

            loginOutput.Member = member.MapTo<MemberDto>();
            loginOutput.Member.OpenId = member.Uid;

            var bindingCustomerId = await _customerDomainService.GetBindingCustomerIdAsync(member.Id);
            loginOutput.Member.HasBindCustomer = bindingCustomerId.HasValue;

            customerId = customerId ?? bindingCustomerId;
            if (customerId.HasValue)
            {
                loginOutput.Member.CustomerId = customerId;
                loginOutput.Member.CustomerName = _nameCacheService.GetCustomerName(customerId);
            }

            if (member.StaffId.HasValue)
            {
                var permissions = await _staffDomainService.GetPermissionsAsync(member.StaffId.Value, SystemType.微信购票系统);
                loginOutput.Permissions.AddRange(permissions);
            }

            if (member.SourceType != ((int)SourceType.售票).ToString())
            {
                loginOutput.Member.IsRegisted = false;
            }
            else
            {
                loginOutput.Member.IsRegisted = true;
            }

            var claims = await BuildMemberClaimsAsync(member, customerId);
            loginOutput.Token = _signInAppService.CreateToken(claims);

            return loginOutput;
        }
    }
}
