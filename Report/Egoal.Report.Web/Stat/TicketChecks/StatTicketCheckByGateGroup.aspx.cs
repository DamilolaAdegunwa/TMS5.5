using Egoal.Report.Extensions;
using Egoal.Report.Tickets;
using Egoal.Report.Tickets.Dto;
using GrapeCity.ActiveReports;
using System;
using System.Data;
using System.Text;

namespace Egoal.Report.Web.Stat.TicketChecks
{
    public partial class StatTicketCheckByGateGroup : PageBase
    {
        private readonly TicketSaleAppService ticketSaleAppService = new TicketSaleAppService();
        private DataTable data = null;

        protected async void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var queryInput = Request.QueryString.ToObject<StatTicketCheckInInput>();
                if (!int.TryParse(Request["height"], out int height))
                {
                    height = 500;
                }

                data = await ticketSaleAppService.StatTicketCheckInByGateGroupAsync(queryInput, Request["token"]);
                if(data.IsNullOrEmpty())
                {
                    WebViewer.Visible = false;
                    Response.Write("<p class='no-data'>暂无数据</p>");
                    return;
                }

                var pageReport = new PageReport(ActiveReportsHelper.GetReport("Tickets.StatTicketCheckByGateGroup.rdlx"));
                var queryString = new StringBuilder($"统计区间:{queryInput.StartCTime}至{queryInput.EndCTime}");
                var parkName = Request["ParkName"];
                if (!string.IsNullOrEmpty(parkName))
                {
                    queryString.Append($",景点:{parkName}");
                }
                var gateGroupName = Request["GateGroupName"];
                if (!string.IsNullOrEmpty(gateGroupName))
                {
                    queryString.Append($",检票点:{gateGroupName}");
                }
                pageReport.Report.ReportParameters[0].DefaultValue.Values.Add(queryString.ToString());
                pageReport.Report.ReportParameters[1].DefaultValue.Values.Add(Request["StaffName"]);
                pageReport.Report.ReportParameters[2].DefaultValue.Values.Add(Request["ScenicName"]);
                pageReport.Report.ReportParameters[3].DefaultValue.Values.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                pageReport.Document.LocateDataSource += Document_LocateDataSource;

                bool.TryParse(Request["isExport"], out bool isExport);
                if (isExport)
                {
                    await ExportToExcelAsync(pageReport.Document, "检票按检票点统计");
                }
                else
                {
                    WebViewer.Visible = true;
                    WebViewer.Report = pageReport;
                    WebViewer.Height = height;
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        private void Document_LocateDataSource(object sender, LocateDataSourceEventArgs args)
        {
            args.Data = data;
        }
    }
}