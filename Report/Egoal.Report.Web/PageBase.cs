using GrapeCity.ActiveReports.Document;
using GrapeCity.ActiveReports.Export.Excel.Page;
using GrapeCity.ActiveReports.Rendering.IO;
using System.Threading.Tasks;
using System;

namespace Egoal.Report.Web
{
    public abstract class PageBase : System.Web.UI.Page
    {
        public int CssVersion = 3;

        protected async Task ExportToExcelAsync(PageDocument pageDocument, string title)
        {
            var excelRenderingExtension = new ExcelRenderingExtension();
            var streamProvider = new MemoryStreamProvider();
            var excelSetting = new ExcelRenderingExtensionSettings();
            excelSetting.FileFormat = FileFormat.Xlsx;
            excelSetting.Pagination = false;
            excelSetting.SheetName = title;
            await Task.Run(() => { pageDocument.Render(excelRenderingExtension, streamProvider, excelSetting.GetSettings()); });

            var streamInfo = streamProvider.GetPrimaryStream();
            var stream = streamInfo.OpenStream();
            var fileContents = new byte[stream.Length];
            await stream.ReadAsync(fileContents, 0, fileContents.Length);
            stream.Dispose();

            Response.Clear();
            Response.ContentType = streamInfo.MimeType;
            Response.AddHeader("Content-Disposition", $"attachment;filename={title}{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
            Response.BinaryWrite(fileContents);
            Response.End();
        }
    }
}