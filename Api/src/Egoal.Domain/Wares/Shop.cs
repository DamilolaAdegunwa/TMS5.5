using Egoal.Domain.Entities;
using System;

namespace Egoal.Wares
{
    public class Shop : Entity
    {
        public string Name { get; set; }
        public int? MerchantId { get; set; }
        public int? SalePointId { get; set; }
        public int? WareHouseId { get; set; }
        public string ShopCode { get; set; }
        public int? ShopTypeId { get; set; }
        public string SortCode { get; set; }
        public Guid SyncCode { get; set; }
    }
}
