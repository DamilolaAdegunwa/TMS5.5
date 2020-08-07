using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Wares.Dto
{
    public class QueryWareIODetailInput : PagedInputDto
    {
        public DateTime? SCTime { get; set; }

        [EndTime]
        public DateTime? ECTime { get; set; }

        public string WareName { get; set; }

        public string ListNo { get; set; }

        public string CzkCardNo { get; set; }

        public int? WareTypeId { get; set; }

        public int? TradeTypeId { get; set; }

        public int? MerchantId { get; set; }

        public int? WareShopId { get; set; }

        public int? CashierId { get; set; }

        public int? CashPcId { get; set; }

    }
}
