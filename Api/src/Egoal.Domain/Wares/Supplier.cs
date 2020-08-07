using Egoal.Domain.Entities;
using System;

namespace Egoal.Wares
{
    public class Supplier : Entity<Guid>
    {
        public string Code { get; set; }
        public string SortCode { get; set; }
        public string Name { get; set; }
        public string Zjf { get; set; }
        public int? SupplierTypeId { get; set; }
        public string SupplierTypeName { get; set; }
        public int? SupplierStatusId { get; set; }
        public string SupplierStatusName { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string Linkman { get; set; }
        public string Memo { get; set; }
        public bool? BlacklistFlag { get; set; }
        public int? Cid { get; set; }
        public DateTime? Ctime { get; set; }
        public int? Mid { get; set; }
        public DateTime? Mtime { get; set; }
        public int? ParkId { get; set; }
        public string ParkName { get; set; }
    }
}
