using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Wares.Dto
{
    public class QueryWareTradeInput : PagedInputDto
    {
        public DateTime? SCTime { get; set; }

        [EndTime]
        public DateTime? ECTime { get; set; }

        public string ListNo { get; set; }

        public string CzkCardNo { get; set; }

        public int? TradeTypeTypeId { get; set; }

        public int? TradeTypeId { get; set; }

        public int? CashierId { get; set; }

        public int? CashPcId { get; set; }

        public int? MerchantId { get; set; }

        public int? ShopTypeId { get; set; }

        public int? ShopId { get; set; }

        public string Memo { get; set; }
    }
}
