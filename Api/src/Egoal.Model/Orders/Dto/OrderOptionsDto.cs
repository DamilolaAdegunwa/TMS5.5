using System.Collections.Generic;

namespace Egoal.Orders.Dto
{
    public class OrderOptionsDto
    {
        public OrderOptionsDto()
        {
            Dates = new List<dynamic>();
            DisabledWeeks = new List<int>();
        }

        public List<dynamic> Dates { get; set; }
        public List<int> DisabledWeeks { get; set; }
        public int GroupOrderMaxQuantity { get; set; }
        public int IndividualOrderMaxAdultQuantity { get; set; }
        public int IndividualOrderMaxChildrenQuantity { get; set; }
        public int PerAdultMaxChildrenQuantity { get; set; }
    }
}
