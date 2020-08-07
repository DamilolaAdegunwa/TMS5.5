using Egoal.Report.Extensions;
using Egoal.Report.Wares;
using Egoal.Report.Wares.Dto;
using GrapeCity.ActiveReports;
using System;
using System.Data;
using System.Text;

namespace Egoal.Report.Web.Stat.Wares
{
    public partial class StatWareSale : PageBase
    {
        private readonly WareAppService WareAppService = new WareAppService();
        private DataTable dataTable = null;

        protected async void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var queryInput = Request.QueryString.ToObject<StatWareSaleInput>();
                if (!int.TryParse(Request["height"], out int height))
                {
                    height = 500;
                }

                dataTable = await WareAppService.StatWareSaleAsync(queryInput, Request["token"]);
                if (dataTable == null || dataTable.Rows.Count < 1)
                {
                    WebViewer.Visible = false;
                    Response.Write("<p class='no-data'>暂无数据</p>");
                    return;
                }
                
                var pageReport = new PageReport(ActiveReportsHelper.GetReport("Wares.StatWareSale.rdlx"));
                var queryString = new StringBuilder($"统计区间:{queryInput.SCTime}至{queryInput.ECTime}");
                if (!string.IsNullOrEmpty(queryInput.WareShopName))
                {
                    queryString.Append($",商店:{queryInput.WareShopName}");
                }
                pageReport.Report.ReportParameters[0].DefaultValue.Values.Add(queryString.ToString());
                pageReport.Report.ReportParameters[1].DefaultValue.Values.Add(Request["ScenicName"]);
                pageReport.Report.ReportParameters[2].DefaultValue.Values.Add(Request["StaffName"]);
                pageReport.Document.LocateDataSource += Document_LocateDataSource;

                bool.TryParse(Request["isExport"], out bool isExport);
                if (isExport)
                {
                    await ExportToExcelAsync(pageReport.Document, "商品销售明细统计");
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
            if (args.DataSetName == "dsWareSale")
            {
                args.Data = dataTable;
            }
        }
    }
}