using Egoal.Domain.Entities;
using System;

namespace Egoal.Wares
{
    public class Merchant : Entity
    {
        public string MerchantName { get; set; }
        public string MerchantCode { get; set; }
        public string SortCode { get; set; }
        public Guid SyncCode { get; set; }
    }
}
