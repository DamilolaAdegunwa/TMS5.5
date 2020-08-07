using Egoal.Report.Extensions;
using Egoal.Report.Wares;
using Egoal.Report.Wares.Dto;
using GrapeCity.ActiveReports;
using System;
using System.Data;
using System.Text;

namespace Egoal.Report.Web.Stat.Wares
{
    public partial class StatWareRentSale : PageBase
    {
        private readonly WareAppService WareAppService = new WareAppService();
        private DataSet dataSet = null;

        protected async void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var queryInput = Request.QueryString.ToObject<StatWareRentSaleInput>();
                if (!int.TryParse(Request["height"], out int height))
                {
                    height = 500;
                }

                dataSet = await WareAppService.StatWareRentSaleAsync(queryInput, Request["token"]);
                if (dataSet == null || dataSet.Tables == null || dataSet.Tables.Count < 2 || (dataSet.Tables[0].Rows.Count < 1 && dataSet.Tables[1].Rows.Count < 1))
                {
                    WebViewer.Visible = false;
                    Response.Write("<p class='no-data'>暂无数据</p>");
                    return;
                }
                
                var pageReport = new PageReport(ActiveReportsHelper.GetReport("Wares.StatWareRentSale.rdlx"));
                var queryString = new StringBuilder($"统计区间:{queryInput.SCTime}至{queryInput.ECTime}");
                if (!string.IsNullOrEmpty(queryInput.WareTypeTypeName))
                {
                    queryString.Append($",商品大类:{queryInput.WareTypeTypeName}");
                }
                if (!string.IsNullOrEmpty(queryInput.WareTypeName))
                {
                    queryString.Append($",商品类型:{queryInput.WareTypeName}");
                }
                if (!string.IsNullOrEmpty(queryInput.MerchantName))
                {
                    queryString.Append($",商家:{queryInput.MerchantName}");
                }
                if (!string.IsNullOrEmpty(queryInput.ShopTypeName))
                {
                    queryString.Append($",商店类型:{queryInput.ShopTypeName}");
                }
                if (!string.IsNullOrEmpty(queryInput.WareShopName))
                {
                    queryString.Append($",商店:{queryInput.WareShopName}");
                }
                if (!string.IsNullOrEmpty(queryInput.SupplierName))
                {
                    queryString.Append($",供应商:{queryInput.SupplierName}");
                }
                if (!string.IsNullOrEmpty(queryInput.CashierName))
                {
                    queryString.Append($",收银员:{queryInput.CashierName}");
                }
                if (!string.IsNullOrEmpty(queryInput.CashPcName))
                {
                    queryString.Append($",收银机:{queryInput.CashPcName}");
                }
                pageReport.Report.ReportParameters[0].DefaultValue.Values.Add(queryString.ToString());
                pageReport.Report.ReportParameters[1].DefaultValue.Values.Add(Request["ScenicName"]);
                pageReport.Report.ReportParameters[2].DefaultValue.Values.Add(Request["StaffName"]);
                pageReport.Report.ReportParameters[3].DefaultValue.Values.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                pageReport.Report.ReportParameters[4].DefaultValue.Values.Add(dataSet.Tables[0].Rows.Count < 1 ? "0" : "1");
                pageReport.Report.ReportParameters[5].DefaultValue.Values.Add(dataSet.Tables[1].Rows.Count < 1 ? "0" : "1");
                pageReport.Document.LocateDataSource += Document_LocateDataSource;

                bool.TryParse(Request["isExport"], out bool isExport);
                if (isExport)
                {
                    await ExportToExcelAsync(pageReport.Document, "商品租售统计");
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
            if (args.DataSetName == "dsWareRent")
            {
                args.Data = dataSet.Tables[0];
            }
            if (args.DataSetName == "dsWareSale")
            {
                args.Data = dataSet.Tables[1];
            }
        }
    }
}