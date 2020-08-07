using Egoal.Domain.Entities;
using System;

namespace Egoal.Customers
{
    public class Promoter : Entity
    {
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Mobile { get; set; }
        public int? CertTypeId { get; set; }
        public string CertNo { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CTime { get; set; } = DateTime.Now;
    }
}
