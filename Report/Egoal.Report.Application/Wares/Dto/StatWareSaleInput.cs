using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Report.Wares.Dto
{
    public class StatWareSaleInput
    {
        public DateTime SCTime { get; set; }

        public DateTime ECTime { get; set; }

        public string WareName { get; set; }

        public int? WareShopId { get; set; }

        public string WareShopName { get; set; }

        public int? CashierId { get; set; }

        public string CashierName { get; set; }

        public int? CashPcId { get; set; }

        public string CashPcName { get; set; }
    }
}
