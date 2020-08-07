using Egoal.Application.Services.Dto;
using Egoal.Tickets.Dto;
using System.Collections.Generic;

namespace Egoal.Orders.Dto
{
    public class OrderDetailDto : EntityDto<long>
    {
        public int? TicketTypeId { get; set; }
        public string TicketTypeName { get; set; }
        public int TotalNum { get; set; }
        public int SurplusNum { get; set; }
        public int UsableQuantity { get; set; }
        public int RefundableQuantity { get; set; }
        public int RefundQuantity { get; set; }
        public bool AllowPartialRefund { get; set; }
        public decimal ReaPrice { get; set; }
        public string ETime { get; set; }
        public string UsageMethod { get; set; }
        public bool ShowQrCode { get; set; }
        public List<TicketSaleSimpleDto> Tickets { get; set; }
        public List<OrderGroundChangCiDto> GroundChangCis { get; set; }
    }
}
