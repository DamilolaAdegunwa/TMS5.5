using System.IO;
using System.Reflection;

namespace Egoal.Report
{
    public static class ActiveReportsHelper
    {
        public static StreamReader GetReport(string reportName)
        {
            return new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream($"Egoal.Report.{reportName}"));
        }
    }
}
