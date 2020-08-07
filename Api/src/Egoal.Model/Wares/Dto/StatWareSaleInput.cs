using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Wares.Dto
{
    public class StatWareSaleInput
    {
        public DateTime? SCTime { get; set; }

        [EndTime]
        public DateTime? ECTime { get; set; }

        public string WareName { get; set; }

        public int? WareShopId { get; set; }

        public int? CashierId { get; set; }

        public int? CashPcId { get; set; }
    }
}
