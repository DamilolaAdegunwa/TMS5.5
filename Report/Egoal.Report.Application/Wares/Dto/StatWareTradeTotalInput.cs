using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Report.Wares.Dto
{
    public class StatWareTradeTotalInput
    {
        public DateTime SCTime { get; set; }

        public DateTime ECTime { get; set; }

        public int? ShopId { get; set; }

        public string ShopName { get; set; }
    }
}
