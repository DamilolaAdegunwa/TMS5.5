using System.Data;
using System.Linq;

namespace Egoal.Report.Extensions
{
    public static class DataTableExtensions
    {
        public static bool IsNullOrEmpty(this DataTable table)
        {
            return table == null || table.Rows.Count <= 0;
        }
    }
}
