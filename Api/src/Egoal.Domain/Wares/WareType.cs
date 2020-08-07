using Egoal.Domain.Entities;

namespace Egoal.Wares
{
    public class WareType : Entity
    {
        public string Name { get; set; }
        public bool NeedWarehouse { get; set; }
        public int? WareTypeTypeId { get; set; }
        public string SortCode { get; set; }
    }
}
