using Egoal.Report.Extensions;
using Egoal.Report.Tickets;
using Egoal.Report.Tickets.Dto;
using GrapeCity.ActiveReports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Egoal.Report.Web.Stat.TicketChecks
{
    public partial class StatTouristNum : PageBase
    {
        private readonly TicketSaleAppService ticketSaleAppService = new TicketSaleAppService();
        private DataTable currentDataSource = null;

        protected async void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var queryInput = Request.QueryString.ToObject<StatTouristNumInput>();
                if (!int.TryParse(Request["height"], out int height))
                {
                    height = 500;
                }

                currentDataSource = await ticketSaleAppService.StatTouristNumAsync(queryInput, Request["token"]);
                if (currentDataSource.IsNullOrEmpty())
                {
                    WebViewer.Visible = false;
                    Response.Write("<p class='no-data'>暂无数据</p>");
                    return;
                }

                var pageReport = new PageReport(ActiveReportsHelper.GetReport("Tickets.StatTouristNum.rdlx"));
                var queryString = new StringBuilder();
                if (queryInput.SSDate.HasValue && queryInput.ESDate.HasValue)
                {
                    queryString.Append($"统计区间:{queryInput.SSDate.Value}至{queryInput.ESDate.Value}");
                }
                
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
                pageReport.Report.ReportParameters[2].DefaultValue.Values.Add(Request["CompanyName"]);
                pageReport.Report.ReportParameters[3].DefaultValue.Values.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                pageReport.Document.LocateDataSource += Document_LocateDataSource;

                bool.TryParse(Request["isExport"], out bool isExport);
                if (isExport)
                {
                    await ExportToExcelAsync(pageReport.Document, "客流统计");
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
            args.Data = currentDataSource;
        }
    }
}