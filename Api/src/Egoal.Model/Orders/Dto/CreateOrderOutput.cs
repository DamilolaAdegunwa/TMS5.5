namespace Egoal.Orders.Dto
{
    public class CreateOrderOutput
    {
        public string ListNo { get; set; }
        public bool ShouldPay { get; set; } = true;
    }
}
