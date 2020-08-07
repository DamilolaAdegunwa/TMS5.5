using Egoal.Domain.Entities;

namespace Egoal.ValueCards
{
    public class CzkCztc : Entity
    {
        public string Name { get; set; }
        public string SortCode { get; set; }
        public decimal? StoredCardMoney { get; set; }
        public decimal? StoredFreeMoney { get; set; }
        public decimal? MaxCouponNum { get; set; }
        public int? MoneyDays { get; set; }
    }
}
