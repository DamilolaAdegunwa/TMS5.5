using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Wares.Dto
{
    public class StatWareSaleByWareTypeInput
    {
        public DateTime? SCTime { get; set; }

        [EndTime]
        public DateTime? ECTime { get; set; }

        public string ListNo { get; set; }

        public int? WareTypeId { get; set; }

        public int? MerchantId { get; set; }

        public int? WareShopId { get; set; }

        public Guid? SupplierId { get; set; }

        public int? CashierId { get; set; }

        public int? CashPcId { get; set; }
    }
}
