using System;

namespace Egoal.Orders.Dto
{
    public class PaySuccessMessage
    {
        public OrderType OrderType { get; set; }
        public Guid MemberId { get; set; }
        public string ListNo { get; set; }
        public decimal TotalMoney { get; set; }
        public string ProductInfo { get; set; }
    }
}
