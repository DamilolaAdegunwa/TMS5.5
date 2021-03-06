using Egoal.Report.Extensions;
using Egoal.Report.Tickets;
using Egoal.Report.Tickets.Dto;
using GrapeCity.ActiveReports;
using System;
using System.Data;
using System.Text;

namespace Egoal.Report.Web.Stat.TicketSales
{
    public partial class StatTicketSaleBySalePoint : PageBase
    {
        private readonly TicketSaleAppService ticketSaleAppService = new TicketSaleAppService();
        private DataTable data = null;

        protected async void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var queryInput = Request.QueryString.ToObject<StatTicketSaleBySalePointInput>();
                if (!int.TryParse(Request["height"], out int height))
                {
                    height = 500;
                }

                data = await ticketSaleAppService.StatTicketSaleBySalePointAsync(queryInput, Request["token"]);
                if (data.IsNullOrEmpty())
                {
                    WebViewer.Visible = false;
                    Response.Write("<p class='no-data'>暂无数据</p>");
                    return;
                }

                var pageReport = new PageReport(ActiveReportsHelper.GetReport("Tickets.StatTicketSaleBySalePoint.rdlx"));
                var queryString = new StringBuilder($"统计区间:{queryInput.StartCTime}至{queryInput.EndCTime}");
                var parkName = Request["ParkName"];
                if (!string.IsNullOrEmpty(parkName))
                {
                    queryString.Append($",景点:{parkName}");
                }
                var salePointName = Request["SalePointName"];
                if (!string.IsNullOrEmpty(salePointName))
                {
                    queryString.Append($",售票点:{salePointName}");
                }
                var ticketTypeName = Request["TicketTypeName"];
                if (!string.IsNullOrEmpty(ticketTypeName))
                {
                    queryString.Append($",票类:{ticketTypeName}");
                }
                pageReport.Report.ReportParameters[0].DefaultValue.Values.Add(queryString.ToString());
                pageReport.Report.ReportParameters[1].DefaultValue.Values.Add(Request["StaffName"]);
                pageReport.Report.ReportParameters[2].DefaultValue.Values.Add(Request["ScenicName"]);
                pageReport.Report.ReportParameters[3].DefaultValue.Values.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                pageReport.Document.LocateDataSource += Document_LocateDataSource;

                bool.TryParse(Request["isExport"], out bool isExport);
                if (isExport)
                {
                    await ExportToExcelAsync(pageReport.Document, "售票按售票点统计");
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