using Egoal.Report.Extensions;
using Egoal.Report.Tickets;
using Egoal.Report.Tickets.Dto;
using Egoal.Report.Trades;
using GrapeCity.ActiveReports;
using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Report.Web.Stat.Shift
{
    public partial class StatShift : PageBase
    {
        private readonly TradeAppService tradeAppService = new TradeAppService();
        private readonly TicketSaleAppService ticketSaleAppService = new TicketSaleAppService();

        StatJbInput queryInput = null;
        DataTable ticketData = null;
        DataTable payDetailData = null;
        DataTable ticketExchangeData = null;
        DataTable czkSaleData = null;
        DataTable wareSaleData = null;

        protected async void Page_Load(object sender, EventArgs e)
        {
            try
            {
                queryInput = Request.QueryString.ToObject<StatJbInput>();
                if (!int.TryParse(Request["height"], out int height))
                {
                    height = 500;
                }

                var ticketTask = ticketSaleAppService.StatTicketSaleJbAsync(queryInput, Request["token"]);
                var payDetailTask = tradeAppService.StatPayDetailJbAsync(queryInput, Request["token"]);
                var ticketExchangeTask = ticketSaleAppService.StatExchangeHistoryJbAsync(queryInput, Request["token"]);
                var czkSaleTask = ticketSaleAppService.StatCzkSaleJbAsync(queryInput, Request["token"]);
                var results = await Task.WhenAll(ticketTask, payDetailTask, ticketExchangeTask, czkSaleTask);
                ticketData = results[0];
                payDetailData = results[1];
                ticketExchangeData = results[2];
                czkSaleData = results[3];
                if (ticketData.IsNullOrEmpty() && payDetailData.IsNullOrEmpty() && ticketExchangeData.IsNullOrEmpty())
                {
                    WebViewer.Visible = false;
                    Response.Write("<p class='no-data'>暂无数据</p>");
                    return;
                }

                var pageReport = new PageReport(ActiveReportsHelper.GetReport("Shift.StatShift.rdlx"));
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
                var cashierName = Request["CashierName"];
                if (!string.IsNullOrEmpty(cashierName))
                {
                    queryString.Append($",收银员:{cashierName}");
                }
                pageReport.Report.ReportParameters[0].DefaultValue.Values.Add(queryString.ToString());
                pageReport.Report.ReportParameters[1].DefaultValue.Values.Add(Request["StaffName"]);
                pageReport.Report.ReportParameters[2].DefaultValue.Values.Add(Request["ScenicName"]);
                pageReport.Report.ReportParameters[3].DefaultValue.Values.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                pageReport.Report.ReportParameters[4].DefaultValue.Values.Add(czkSaleData.IsNullOrEmpty() ? "0" : "1");
                pageReport.Report.ReportParameters[5].DefaultValue.Values.Add(queryInput.IncludeWareDetail && !wareSaleData.IsNullOrEmpty() ? "1" : "0");
                pageReport.Report.ReportParameters[6].DefaultValue.Values.Add(queryInput.StatTicketByPayType && !ticketData.IsNullOrEmpty() ? "1" : "0");
                pageReport.Report.ReportParameters[7].DefaultValue.Values.Add(payDetailData.IsNullOrEmpty() ? "0" : "1");
                pageReport.Report.ReportParameters[8].DefaultValue.Values.Add(!queryInput.IncludeWareDetail && !wareSaleData.IsNullOrEmpty() ? "1" : "0");
                pageReport.Report.ReportParameters[9].DefaultValue.Values.Add(!queryInput.StatTicketByPayType && !ticketData.IsNullOrEmpty() ? "1" : "0");
                pageReport.Report.ReportParameters[10].DefaultValue.Values.Add(ticketExchangeData.IsNullOrEmpty() ? "0" : "1");
                pageReport.Document.LocateDataSource += Document_LocateDataSource;

                bool.TryParse(Request["isExport"], out bool isExport);
                if (isExport)
                {
                    await ExportToExcelAsync(pageReport.Document, "交班统计");
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
            if (args.DataSetName == "dsTicketSale")
            {
                args.Data = queryInput.StatTicketByPayType ? new DataTable() : ticketData;
            }
            if (args.DataSetName == "dsTicketSaleDetail")
            {
                args.Data = queryInput.StatTicketByPayType ? ticketData : new DataTable();
            }
            if (args.DataSetName == "dsPayDetail")
            {
                args.Data = payDetailData;
            }
            if (args.DataSetName == "dsTicketExchange")
            {
                args.Data = ticketExchangeData;
            }
            if (args.DataSetName == "dsCzkSale")
            {
                args.Data = czkSaleData;
            }
            if (args.DataSetName == "dsWareSale")
            {
                args.Data = new DataTable();
            }
            if (args.DataSetName == "dsWareSaleDetail")
            {
                args.Data = new DataTable();
            }
        }
    }
}