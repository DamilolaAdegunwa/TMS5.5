using Egoal.Domain.Entities.Auditing;
using Egoal.Extensions;
using System;
using System.Collections.Generic;

namespace Egoal.TicketTypes
{
    public class TicketType : AuditedEntity
    {
        public TicketType()
        {
            TicketTypeGrounds = new List<TicketTypeGround>();
        }

        public TicketTypeType? TicketTypeTypeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Zjf { get; set; }
        public string SortCode { get; set; }
        public int? TicketTypeProjectId { get; set; }
        public decimal? TicPrice { get; set; }
        public decimal NetPrice { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? PrintPrice { get; set; }
        public decimal? DiscountRate { get; set; }
        public int? MaxSaleNumByDay { get; set; }
        public decimal? CheckPrice { get; set; }
        public decimal? GbFee { get; set; }
        public decimal? HkFee { get; set; }
        public decimal? XkFee { get; set; }
        public int? DelayDays { get; set; }
        public decimal? DelayFee { get; set; }
        public int? DelayTypeId { get; set; }
        public int? FingerPrintDays { get; set; }
        public decimal? RentDeviceFee { get; set; }
        public decimal? YaJin { get; set; }
        public decimal? StoredCardMoney { get; set; }
        public decimal? StoredFreeMoney { get; set; }
        public decimal? StoredGameMoney { get; set; }
        public decimal? CardMoneyRate { get; set; }
        public decimal? Overdraw { get; set; }
        public decimal? WarnMoney { get; set; }
        public int? MoneyDays { get; set; }
        public decimal? MinRechargeMoney { get; set; }
        public int? Days { get; set; }
        public int? Minutes { get; set; }
        public int? FeeUnit { get; set; }
        public decimal? MinFee { get; set; }
        public decimal? MaxFee { get; set; }
        public int? JfUnit { get; set; }
        public int? Jf { get; set; }
        public int? MaxJf { get; set; }
        public string SvalidDate { get; set; }
        public string ValidDate { get; set; }
        public int? BookDays { get; set; } = 30;
        public string BookSdate { get; set; }
        public string BookEdate { get; set; }
        public int? CheckGroundNum { get; set; }
        public int? CheckNum { get; set; }
        public int? PersonNum { get; set; }
        public int? FreeMinutes { get; set; }
        public int? MaxCheckNumByDay { get; set; }
        public CheckType CheckTypeId { get; set; }
        public int? TicketTypeGoRuleTypeId { get; set; }
        public int? GlkGoTypeId { get; set; }
        public int? GnkTypeId { get; set; }
        public int? GnkGoTypeId { get; set; }
        public bool? RecycleFlag { get; set; }
        public bool? AutoCheckFlag { get; set; }
        public bool? FirstActiveFlag { get; set; }
        public bool? SecondActiveFlag { get; set; }
        public bool? FingerFlag { get; set; }
        public int? FingerBindTypeId { get; set; }
        public bool? VerifyFaceFlag { get; set; }
        public string Stime { get; set; }
        public string Etime { get; set; }
        public int? CheckInterval { get; set; }
        public int? EarlyIn { get; set; }
        public int? DelayIn { get; set; }
        public int? Timeout { get; set; }
        public decimal? TimeoutFee { get; set; }
        public int? VoiceId { get; set; }
        public int? LedId { get; set; }
        public int? LcdId { get; set; }
        public string LcdName { get; set; }
        public int? TicketBindTypeId { get; set; }
        public TicketKind? Tkid { get; set; }
        public int? Ttid { get; set; }
        public int? ExchangeTicketTypeId { get; set; }
        public bool? CheckNumAsPersonNumFlag { get; set; }
        public int? DefaultPayTypeId { get; set; }
        public bool? PrintFlag { get; set; }
        public int? PrinterId { get; set; }
        public bool? SaleFlag { get; set; }
        public bool? StatFlag { get; set; }
        public bool? StatTicketCheckFlag { get; set; }
        public bool? JbStatFlag { get; set; }
        public bool? StoreMoneyFlag { get; set; }
        public bool? DiscountFlag { get; set; }
        public bool? PointFlag { get; set; }
        public bool? SalePermitFlag { get; set; }
        public TouristInfoType? TouristInfoType { get; set; }
        public bool? NeedTouristName { get; set; }
        public bool? NeedTouristMobile { get; set; }
        public bool? NeedCertFlag { get; set; }
        public int? AuthorizationTypeId { get; set; }
        public int? TicketTypeStatTypeId { get; set; }
        public bool? BdFlag { get; set; }
        public bool? TcFlag { get; set; }
        public bool? CouponFlag { get; set; }
        public bool? GroupTicketFlag { get; set; }
        public bool? ExpireToNextYearFlag { get; set; }
        public bool? CabinetCardFlag { get; set; }
        public bool? PublicSaleFlag { get; set; }
        public int? XsTypeId { get; set; }
        public int? ApplyToSystemTypeId { get; set; }
        public int? SaleSiteId { get; set; }
        public string ImgPath { get; set; }
        public bool? AllowSecondIn { get; set; }
        public string LastBookTime { get; set; }
        public int? AdvanceBookDays { get; set; }
        public int? MinBuyNum { get; set; }
        public int? MaxExchangeDays { get; set; }
        public string TheTicketDate { get; set; }
        public bool AllowRefund { get; set; }
        public bool? AllowExpiredRefund { get; set; }
        public int? AllowExpiredRefundMaxDays { get; set; }
        public bool AllowPartialRefund { get; set; }
        public bool NeedContactFlag { get; set; }
        public int? MaxBuyNum { get; set; }
        public int MemberLimitDays { get; set; }
        public int MemberLimitCount { get; set; }
        public string Detail { get; set; }
        public bool? ShouldReadDescription { get; set; } = false;
        public bool ShouldPrintAfterCheck { get; set; }
        public string Memo { get; set; }
        public int? ParkId { get; set; }
        public Guid SyncCode { get; set; }
        public int? OTATicketPutOffMinutes { get; set; }
        public string UsageMethod { get; set; }
        public bool WxShowQrCode { get; set; }

        public virtual ICollection<TicketTypeGround> TicketTypeGrounds { get; set; }
        public virtual ICollection<TicketTypeGroundSharing> TicketTypeGroundSharings { get; set; }
        public virtual TicketTypeDescription TicketTypeDescription { get; set; }

        public DateTime GetStartTravelDate(SaleChannel saleChannel)
        {
            var startTravelDate = DateTime.Now.Date;
            if (saleChannel == SaleChannel.Net)
            {
                if (AdvanceBookDays > 0)
                {
                    startTravelDate = DateTime.Now.Date.AddDays(AdvanceBookDays.Value);
                }
                else if (!LastBookTime.IsNullOrEmpty())
                {
                    var lastBookTime = $"{DateTime.Now.ToDateString()} {LastBookTime}:00".To<DateTime>();
                    if (DateTime.Now > lastBookTime)
                    {
                        startTravelDate = DateTime.Now.Date.AddDays(1);
                    }
                }
            }
            if (DateTime.TryParse(SvalidDate, out DateTime startValidDate))
            {
                startTravelDate = DateTimeExtensions.Max(startTravelDate, startValidDate);
            }

            return startTravelDate;
        }

        public DateTime GetEndTravelDate(SaleChannel saleChannel)
        {
            var endValidDate = Convert.ToDateTime(ValidDate);
            if (saleChannel == SaleChannel.Net && BookDays > 0)
            {
                var startTravelDate = GetStartTravelDate(saleChannel);
                var endTravelDate = startTravelDate.AddDays(BookDays.Value - 1);
                return DateTimeExtensions.Min(endTravelDate, endValidDate);
            }
            return endValidDate;
        }

        public bool IsRefundLimited()
        {
            return AllowExpiredRefund == false ||
                (AllowExpiredRefund == true && AllowExpiredRefundMaxDays > 0) ||
                AllowPartialRefund == false;
        }

        public string GetDisplayName()
        {
            if (FullName.IsNullOrEmpty())
            {
                return Name;
            }

            return FullName;
        }

        public int GetMaxBuyNum()
        {
            int maxBuyNum = MaxBuyNum ?? 1000;
            if (MemberLimitDays > 0 && MemberLimitCount > 0)
            {
                maxBuyNum = Math.Min(maxBuyNum, MemberLimitCount);
            }
            return maxBuyNum;
        }
    }
}
