using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Wares.Dto
{
    public class StatWareTradeTotalInput
    {
        public DateTime? SCTime { get; set; }

        [EndTime]
        public DateTime? ECTime { get; set; }

        public int? ShopId { get; set; }
    }
}
