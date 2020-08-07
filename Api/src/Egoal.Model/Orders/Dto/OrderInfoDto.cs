using Egoal.Tickets.Dto;
using System.Collections.Generic;

namespace Egoal.Orders.Dto
{
    public class OrderInfoDto
    {
        public OrderInfoDto()
        {
            Details = new List<OrderDetailDto>();
            Tourists = new List<TouristDto>();
        }

        public string ListNo { get; set; }
        public string OrderTypeName { get; set; }
        public string OrderStatusName { get; set; }
        public string TravelDate { get; set; }
        public bool ShouldPay { get; set; }
        public decimal TotalMoney { get; set; }
        public string PayTypeName { get; set; }
        public int PersonNum { get; set; }
        public string CTime { get; set; }
        public bool AllowCancel { get; set; }
        public bool ShowFace { get; set; }
        public string ConsumeStatusName { get; set; }
        public string RefundStatusName { get; set; }
        public string ArrivalTime { get; set; }
        public string LicensePlateNumber { get; set; }
        public string JidiaoName { get; set; }
        public string KeYuanTypeName { get; set; }
        public string AreaName { get; set; }
        public string CustomerName { get; set; }
        public string MemberName { get; set; }
        public string GuiderName { get; set; }
        public string PromoterName { get; set; }
        public string ThirdListNo { get; set; }
        public string CashierName { get; set; }
        public string ExplainerName { get; set; }
        public string ExplainerTime { get; set; }
        public long ExpireSeconds { get; set; }
        public object Contact { get; set; }
        public string Memo { get; set; }

        public List<OrderDetailDto> Details { get; set; }
        public List<TouristDto> Tourists { get; set; }
    }
}
