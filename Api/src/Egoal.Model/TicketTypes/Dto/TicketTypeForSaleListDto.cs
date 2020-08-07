using Egoal.Application.Services.Dto;
using Egoal.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Egoal.TicketTypes.Dto
{
    public class TicketTypeForSaleListDto : EntityDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        [JsonConverter(typeof(DateConverter))]
        public DateTime StartTravelDate { get; set; }
        public bool AllowRefund { get; set; }
        public bool RefundLimited { get; set; }
        public bool ShouldReadDescription { get; set; }
        public List<string> Classes { get; set; }
    }
}
