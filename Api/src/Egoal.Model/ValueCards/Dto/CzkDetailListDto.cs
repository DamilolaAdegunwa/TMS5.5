using Egoal.Extensions;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Egoal.ValueCards.Dto
{
    public class CzkDetailListDto
    {
        [Display(Name = "序号")]
        public string RowNum { get; set; }

        [Display(Name = "单号")]
        public string ListNo { get; set; }

        [Display(Name = "交易时间")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Ctime { get; set; }

        [JsonIgnore]
        public CzkOpType? CzkOpTypeId { get; set; }

        [Display(Name = "操作类型")]
        public string CzkOpTypeName { get; set; }

        [Display(Name = "票号")]
        public string TicketCode { get; set; }

        [Display(Name = "卡号")]
        public string CardNo { get; set; }

        [JsonIgnore]
        public int? TicketTypeId { get; set; }

        [Display(Name = "卡类")]
        public string TicketTypeName { get; set; }

        [JsonIgnore]
        public CzkRechargeType? CzkRechargeTypeId { get; set; }

        [Display(Name = "充值类型")]
        public string CzkRechargeTypeName { get; set; }

        [JsonIgnore]
        public int? CzkCztcId { get; set; }

        [Display(Name = "充值套餐")]
        public string CzkCztcName { get; set; }

        [JsonIgnore]
        public CzkConsumeType? CzkConsumeTypeId { get; set; }

        [Display(Name = "消费类型")]
        public string CzkConsumeTypeName { get; set; }

        [Display(Name = "原本金")]
        public decimal? OldCardMoney { get; set; }

        [Display(Name = "原赠送金")]
        public decimal? OldFreeMoney { get; set; }

        [Display(Name = "原体验金")]
        public decimal? OldGameMoney { get; set; }

        [Display(Name = "原总金额")]
        public decimal? OldTotalMoney { get; set; }

        [Display(Name = "充值本金")]
        public decimal? RechargeCardMoney { get; set; }

        [Display(Name = "充值赠送金")]
        public decimal? RechargeFreeMoney { get; set; }

        [Display(Name = "充值体验金")]
        public decimal? RechargeGameMoney { get; set; }

        [Display(Name = "充值总金额")]
        public decimal? RechargeTotalMoney { get; set; }

        [Display(Name = "优惠券面额")]
        public decimal? UseCouponNum { get; set; }

        [Display(Name = "消费本金")]
        public decimal? ConsumeCardMoney { get; set; }

        [Display(Name = "消费赠送金")]
        public decimal? ConsumeFreeMoney { get; set; }

        [Display(Name = "消费体验金")]
        public decimal? ConsumeGameMoney { get; set; }

        [Display(Name = "消费总金额")]
        public decimal? ConsumeTotalMoney { get; set; }

        [Display(Name = "新本金")]
        public decimal? NewCardMoney { get; set; }

        [Display(Name = "新赠送金")]
        public decimal? NewFreeMoney { get; set; }

        [Display(Name = "新体验金")]
        public decimal? NewGameMoney { get; set; }

        [Display(Name = "新总金额")]
        public decimal? NewTotalMoney { get; set; }

        [Display(Name = "押金")]
        public decimal? YaJin { get; set; }

        [JsonIgnore]
        public Guid? MemberId { get; set; }

        [Display(Name = "会员")]
        public string MemberName { get; set; }

        [JsonIgnore]
        public int? PayTypeId { get; set; }

        [Display(Name = "付款方式")]
        public string PayTypeName { get; set; }

        [JsonIgnore]
        public int? CashierId { get; set; }

        [JsonIgnore]
        public int? GroundId { get; set; }

        [Display(Name = "收银")]
        public string CashierName { get; set; }

        [Display(Name = "备注")]
        public string Memo { get; set; }
    }
}
