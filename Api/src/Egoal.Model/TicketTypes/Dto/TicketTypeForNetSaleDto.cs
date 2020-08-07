using Egoal.Application.Services.Dto;
using Egoal.Extensions;
using Egoal.Scenics.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Egoal.TicketTypes.Dto
{
    public class TicketTypeForNetSaleDto : EntityDto
    {
        public TicketTypeForNetSaleDto()
        {
            DailyPrices = new List<dynamic>();
        }

        public string Name { get; set; }
        public TicketTypeType TicketTypeType { get; set; }
        public List<dynamic> DailyPrices { get; set; }

        [JsonConverter(typeof(DateConverter))]
        public DateTime StartTravelDate { get; set; }
        public bool AllowRefund { get; set; }
        public bool RefundLimited { get; set; }
        public int MinBuyNum { get; set; }
        public int MaxBuyNum { get; set; }
        public TouristInfoType TouristInfoType { get; set; }
        public bool NeedTouristName { get; set; }
        public bool NeedTouristMobile { get; set; }
        public bool NeedCert { get; set; }
        public bool WxTouristNeedCertTypeFlag { get; set; }
        public int MemberLimitDays { get; set; }
        public List<GroundChangCisDto> GroundChangCis { get; set; }
        public bool HasGroundChangCi
        {
            get
            {
                return !GroundChangCis.IsNullOrEmpty();
            }
        }
    }
}
