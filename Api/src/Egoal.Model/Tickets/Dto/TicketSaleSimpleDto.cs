namespace Egoal.Tickets.Dto
{
    public class TicketSaleSimpleDto
    {
        public string TicketCode { get; set; }
        public string TicketStatusName { get; set; }
        public int TicketTypeId { get; set; }
        public string TicketTypeName { get; set; }
        public int Quantity { get; set; }
        public int SurplusQuantity { get; set; }
        public int TotalNum { get; set; }
        public string Stime { get; set; }
        public string Etime { get; set; }
        public long? OrderDetailId { get; set; }
        public bool ShowFace { get; set; }
        public bool IsUsable { get; set; }
        public bool AllowRefund { get; set; }
    }
}
