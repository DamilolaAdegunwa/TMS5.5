using Egoal.Auditing;
using Egoal.Authorization;
using Egoal.BackgroundJobs;
using Egoal.Common;
using Egoal.Customers;
using Egoal.Domain.Uow;
using Egoal.DynamicCodes;
using Egoal.EntityFrameworkCore.Mappings;
using Egoal.Events.Bus.Entities;
using Egoal.Members;
using Egoal.Messages;
using Egoal.Orders;
using Egoal.Payment;
using Egoal.Runtime.Session;
using Egoal.Scenics;
using Egoal.Settings;
using Egoal.Stadiums;
using Egoal.Staffs;
using Egoal.ThirdPlatforms;
using Egoal.Tickets;
using Egoal.TicketTypes;
using Egoal.Trades;
using Egoal.ValueCards;
using Egoal.Wares;
using Microsoft.EntityFrameworkCore;

namespace Egoal
{
    public class TmsDbContext : DbContextBase
    {
        #region Auditing
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        #endregion

        #region Authorization
        public virtual DbSet<Right> Rights { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        #endregion

        #region BackgroundJobs
        public virtual DbSet<AbpBackgroundJob> AbpBackgroundJobs { get; set; }
        public virtual DbSet<BackgroundJobInfo> BackgroundJobs { get; set; }
        #endregion

        #region Common
        public virtual DbSet<AgeRange> AgeRanges { get; set; }
        public virtual DbSet<ApiLog> ApiLogs { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<CertType> CertTypes { get; set; }
        public virtual DbSet<ChangCi> ChangCis { get; set; }
        public virtual DbSet<KeYuanType> KeYuanTypes { get; set; }
        public virtual DbSet<TmDate> TmDates { get; set; }
        #endregion

        #region Customers
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerMemberBind> CustomerMemberBinds { get; set; }
        public virtual DbSet<CustomerPhoto> CustomerPhotos { get; set; }
        public virtual DbSet<CustomerType> CustomerTypes { get; set; }
        public virtual DbSet<Promoter> Promoters { get; set; }
        #endregion

        #region DynamicCodes
        public virtual DbSet<ListNo> ListNos { get; set; }
        #endregion

        #region Members
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<MemberCard> MemberCards { get; set; }
        public virtual DbSet<MemberPhoto> MemberPhotos { get; set; }
        public virtual DbSet<UserWechat> UserWechats { get; set; }
        #endregion

        #region Messages
        public virtual DbSet<WeChatMessageTemplate> WeChatMessageTemplates { get; set; }
        #endregion

        #region Orders
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderAgeRange> OrderAgeRanges { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderGroundChangCi> OrderGroundChangCis { get; set; }
        public virtual DbSet<OrderPlan> OrderPlans { get; set; }
        public virtual DbSet<OrderStat> OrderStats { get; set; }
        public virtual DbSet<OrderTourist> OrderTourists { get; set; }
        public virtual DbSet<RefundOrderApply> RefundOrderApplies { get; set; }
        #endregion

        #region Payment
        public virtual DbSet<NetPayOrder> NetPayOrders { get; set; }
        public virtual DbSet<NetPayRefundFail> NetPayRefundFails { get; set; }
        public virtual DbSet<PayType> PayTypes { get; set; }
        public virtual DbSet<RefundMoneyApply> RefundMoneyApplies { get; set; }
        #endregion

        #region Scenics
        public virtual DbSet<Gate> Gates { get; set; }
        public virtual DbSet<GateGroup> GateGroups { get; set; }
        public virtual DbSet<Ground> Grounds { get; set; }
        public virtual DbSet<GroundChangCiPlan> GroundChangCiPlans { get; set; }
        public virtual DbSet<GroundDateChangCiSaleNum> GroundDateChangCiSaleNums { get; set; }
        public virtual DbSet<GroundRemoteBookRecord> GroundRemoteBookRecords { get; set; }
        public virtual DbSet<Park> Parks { get; set; }
        public virtual DbSet<Pc> Pcs { get; set; }
        public virtual DbSet<SalePoint> SalePoints { get; set; }
        public virtual DbSet<Scenic> Scenics { get; set; }
        #endregion

        #region Settings
        public virtual DbSet<Constant> Constants { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<TimePoint> TimePoints { get; set; }
        #endregion

        #region Stadiums
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Seat> Seats { get; set; }
        public virtual DbSet<SeatStatusCache> SeatStatusCaches { get; set; }
        public virtual DbSet<Stadium> Stadiums { get; set; }
        #endregion

        #region Staffs
        public virtual DbSet<ExplainerTimeslot> ExplainerTimeslots { get; set; }
        public virtual DbSet<ExplainerTimeslotScheduling> ExplainerTimeslotSchedulings { get; set; }
        public virtual DbSet<ExplainerWorkRecord> ExplainerWorkRecords { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        #endregion

        #region ThirdPlatforms
        public virtual DbSet<ThirdPlatform> ThirdPlatforms { get; set; }
        #endregion

        #region Tickets
        public virtual DbSet<TicketCheck> TicketChecks { get; set; }
        public virtual DbSet<TicketCheckDayStat> TicketCheckDayStats { get; set; }
        public virtual DbSet<TicketConsume> TicketConsumes { get; set; }
        public virtual DbSet<TicketExchangeHistory> TicketExchangeHistories { get; set; }
        public virtual DbSet<TicketGround> TicketGrounds { get; set; }
        public virtual DbSet<TicketGroundCache> TicketGroundCaches { get; set; }
        public virtual DbSet<TicketGroundSale> TicketGroundSales { get; set; }
        public virtual DbSet<TicketGroundType> TicketGroundTypes { get; set; }
        public virtual DbSet<TicketReprintLog> TicketReprintLogs { get; set; }
        public virtual DbSet<TicketSale> TicketSales { get; set; }
        public virtual DbSet<TicketSaleBuyer> TicketSaleBuyers { get; set; }
        public virtual DbSet<TicketSaleDayStat> TicketSaleDayStats { get; set; }
        public virtual DbSet<TicketSaleGroundSharing> TicketSaleGroundSharings { get; set; }
        public virtual DbSet<TicketSalePhoto> TicketSalePhotos { get; set; }
        public virtual DbSet<TicketSalePhotoQueque> TicketSalePhotoQueques { get; set; }
        public virtual DbSet<TicketSaleSeat> TicketSaleSeats { get; set; }
        public virtual DbSet<TicketSaleStock> TicketSaleStocks { get; set; }
        #endregion

        #region TicketTypes
        public virtual DbSet<TicketType> TicketTypes { get; set; }
        public virtual DbSet<TicketTypeClass> TicketTypeClasses { get; set; }
        public virtual DbSet<TicketTypeClassDetail> TicketTypeClassDetails { get; set; }
        public virtual DbSet<TicketTypeDateTypePrice> TicketTypeDateTypePrices { get; set; }
        public virtual DbSet<TicketTypeDescription> TicketTypeDescriptions { get; set; }
        public virtual DbSet<TicketTypeGateGroup> TicketTypeGateGroups { get; set; }
        public virtual DbSet<TicketTypeGround> TicketTypeGrounds { get; set; }
        public virtual DbSet<TicketTypeGroundChangCi> TicketTypeGroundChangCis { get; set; }
        public virtual DbSet<TicketTypeGroundPrice> TicketTypeGroundPrices { get; set; }
        public virtual DbSet<TicketTypeGroundSharing> TicketTypeGroundSharings { get; set; }
        public virtual DbSet<TicketTypeGroundType> TicketTypeGroundTypes { get; set; }
        public virtual DbSet<TicketTypeStock> TicketTypeStocks { get; set; }
        #endregion

        #region Trades
        public virtual DbSet<PayDetail> PayDetails { get; set; }
        public virtual DbSet<Trade> Trades { get; set; }
        public virtual DbSet<TradeDetail> TradeDetails { get; set; }
        public virtual DbSet<TradeType> TradeTypes { get; set; }
        #endregion

        #region ValueCards
        public virtual DbSet<CzkCztc> CzkCztcs { get; set; }
        public virtual DbSet<CzkDetail> CzkDetails { get; set; }
        public virtual DbSet<MemberAccount> MemberAccounts { get; set; }
        public virtual DbSet<TicketBlacklistCheck> TicketBlacklistChecks { get; set; }
        #endregion

        #region Wares
        public virtual DbSet<Merchant> Merchants { get; set; }
        public virtual DbSet<Shop> Shops { get; set; }
        public virtual DbSet<ShopType> ShopTypes { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<WareType> WareTypes { get; set; }
        public virtual DbSet<WareTypeType> WareTypeTypes { get; set; }
        public virtual DbSet<Ware> Wares { get; set; }
        public virtual DbSet<WareIODetail> WareIoDetails { get; set; }
        #endregion

        public TmsDbContext(
            DbContextOptions<TmsDbContext> options,
            ISession session,
            IEntityChangeEventHelper entityChangeEventHelper,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(options, session, entityChangeEventHelper, currentUnitOfWorkProvider)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            TmsMapping.ApplyConfiguration(modelBuilder);
        }
    }
}
