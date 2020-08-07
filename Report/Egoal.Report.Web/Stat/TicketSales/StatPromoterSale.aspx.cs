using Egoal.Report.Extensions;
using Egoal.Report.Tickets;
using Egoal.Report.Tickets.Dto;
using GrapeCity.ActiveReports;
using System;
using System.Data;
using System.Text;

namespace Egoal.Report.Web.Stat.TicketSales
{
    public partial class StatPromoterSale : PageBase
    {
        private readonly TicketSaleAppService ticketSaleAppService = new TicketSaleAppService();
        private DataTable data = null;

        protected async void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var queryInput = Request.QueryString.ToObject<StatPromoterSaleInput>();
                if (!int.TryParse(Request["height"], out int height))
                {
                    height = 500;
                }

                data = await ticketSaleAppService.StatPromoterSaleAsync(queryInput, Request["token"]);
                if (data.IsNullOrEmpty())
                {
                    WebViewer.Visible = false;
                    Response.Write("<p class='no-data'>暂无数据</p>");
                    return;
                }

                var groundRefunds = data.Select("type='项目退费'");
                var reportName = groundRefunds != null && groundRefunds.Length > 0 ? "Tickets.StatPromoterSale.rdlx" : "Tickets.StatPromoterSaleWithoutType.rdlx";
                var pageReport = new PageReport(ActiveReportsHelper.GetReport(reportName));

                var queryText = new StringBuilder();
                queryText.Append($"销售时间：{queryInput.StartCTime.ToString("yyyy-MM-dd HH:mm:ss")}至{queryInput.EndCTime.ToString("yyyy-MM-dd HH:mm:ss")}，");

                pageReport.Report.ReportParameters[0].DefaultValue.Values.Add(Request["ScenicName"]);
                pageReport.Report.ReportParameters[1].DefaultValue.Values.Add($"制表时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}");
                pageReport.Report.ReportParameters[2].DefaultValue.Values.Add($"    制表人：{Request["StaffName"]}");
                pageReport.Report.ReportParameters[3].DefaultValue.Values.Add(queryText.ToString().TrimEnd('，'));
                pageReport.Document.LocateDataSource += Document_LocateDataSource;

                bool.TryParse(Request["isExport"], out bool isExport);
                if (isExport)
                {
                    await ExportToExcelAsync(pageReport.Document, "营销推广员销售汇总");
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