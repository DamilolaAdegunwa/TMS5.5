using Egoal.Report.Extensions;
using Egoal.Report.Tickets;
using Egoal.Report.Tickets.Dto;
using GrapeCity.ActiveReports.Viewer.Win;
using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.Design;
using O2S.Components.PDFRender4NET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;

namespace Egoal.Report.Web.Stat.Template
{
    /// <summary>
    /// TicketTemplate 的摘要说明
    /// </summary>
    public class TicketTemplate : System.Web.HttpTaskAsyncHandler
    {
        private QrCodeHelper _qrCodeHelper = new QrCodeHelper();
        string physicalApplicationPath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
        private readonly TicketSaleAppService ticketSaleAppService = new TicketSaleAppService();
        DataTable ticketDataTable = new DataTable();
        TicketTemplatePrintInput ticketTemplatePrintInput;

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            ticketTemplatePrintInput = context.Request.Params.ToObject<TicketTemplatePrintInput>();
            string chen = context.Request.Params["ticketTypeName"];
            string dian = context.Request.Params["companyName"];
            //ticketTemplatePrintInput = System.Web.HttpRequest.QueryString.ToObject<TicketTemplatePrintInput>();
            context.Response.ClearHeaders();
            context.Response.AppendHeader("Access-Control-Allow-Headers", "Content-Type,Content-Length, Authorization, Accept,X-Requested-With");
            context.Response.AppendHeader("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS");
            context.Response.AppendHeader("X-Powered-By", "3.2.1");
            context.Response.ContentType = "application/json";
            //context.Response.ContentType = "application/x-www-form-urlencoded";
            string msg = string.Format(@"接口访问成功");

            string errorMessage = "";
            byte[] imageBytes = GetImageByte();
            dynamic resultDynamic = new
            {
                msg = "接口访问成功",
                imageBytes = imageBytes
            };
            msg = javaScriptSerializer.Serialize(imageBytes);
            string result = "[{\"Result\":\"" + msg + "\"}]";

            context.Response.Write(msg);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public byte[] ExportPdfToBytes(int printWidth, int printHeight, ref string errorMessage)
        {
            byte[] imageBytes = null;

            string template = "﻿<?xml version=\"1.0\" encoding=\"utf-8\"?><ActiveReportsLayout Version=\"3.2\" PrintWidth=\"5555.906\" DocumentName=\"ActiveReports Document\" ScriptLang=\"C#\" MasterReport=\"0\"><StyleSheet><Style Name=\"Normal\" Value=\"font-family: Arial; font-style: normal; text-decoration: none; font-weight: normal; font-size: 10pt; color: Black; text-align: left; vertical-align: top; ddo-char-set: 1\" /><Style Name=\"Heading1\" Value=\"font-family: Arial; font-size: 16pt; font-style: normal; font-weight: bold\" /><Style Name=\"Heading2\" Value=\"font-family: Times New Roman; font-size: 14pt; font-style: italic; font-weight: bold\" /><Style Name=\"Heading3\" Value=\"font-family: Arial; font-size: 13pt; font-style: normal; font-weight: bold\" /></StyleSheet><Sections><Section Type=\"Detail\" Name=\"Detail\" Height=\"3288.189\" BackColor=\"16777215\"><Control Type=\"AR.Label\" Name=\"Label1\" Left=\"566.9294\" Top=\"765.3541\" Width=\"1062.992\" Height=\"318.6142\" Caption=\"票类名称：\" Style=\"font-family: \\005FAE\\008F6F\\0096C5\\009ED1; font-size: 11.25pt; text-align: justify; vertical-align: top; white-space: nowrap; ddo-char-set: 134; ddo-wrap-mode: nowrap\" Multiline=\"0\" /><Control Type=\"AR.Label\" Name=\"Label3\" Left=\"560.1263\" Top=\"2061.355\" Width=\"1083.402\" Height=\"323.1487\" Caption=\"人      次：\" Style=\"font-family: \\005FAE\\008F6F\\0096C5\\009ED1; font-size: 11.25pt; text-align: justify; vertical-align: top; white-space: nowrap; ddo-char-set: 134; ddo-wrap-mode: nowrap\" Multiline=\"0\" /><Control Type=\"AR.Label\" Name=\"Label4\" Left=\"573.7327\" Top=\"1083.969\" Width=\"1069.795\" Height=\"283.4645\" Caption=\"购票日期：\" Style=\"font-family: \\005FAE\\008F6F\\0096C5\\009ED1; font-size: 11.25pt; text-align: justify; vertical-align: top; white-space: nowrap; ddo-char-set: 134; ddo-wrap-mode: nowrap\" Multiline=\"0\" /><Control Type=\"AR.Label\" Name=\"Label5\" Left=\"565.7955\" Top=\"1367.433\" Width=\"1069.795\" Height=\"319.1811\" Caption=\"有效期至：\" Style=\"font-family: \\005FAE\\008F6F\\0096C5\\009ED1; font-size: 11.25pt; text-align: justify; vertical-align: top; white-space: nowrap; ddo-char-set: 134; ddo-wrap-mode: nowrap\" Multiline=\"0\" /><Control Type=\"AR.Barcode\" Name=\"Barcode1\" DataField=\"ticketcode\" Left=\"3968.504\" Top=\"637.7953\" Width=\"907\" Height=\"907\" BarStyle=\"24\" Text=\"123456789012\" QRCode=\"EncodingCodePage=65001\" MicroQRCode=\"EncodingCodePage=65001\" PDF417=\"EncodingCodePage=65001\" MicroPDF417=\"EncodingCodePage=65001\" CODE128=\"0;0;0\" DataMatrix=\"EncodingCodePage=65001\" /><Control Type=\"AR.Field\" Name=\"TextBox1\" DataField=\"TicketTypeFullName\" Left=\"1700.787\" Top=\"765.3541\" Width=\"1440\" Height=\"318.6142\" Text=\"国艺影城-全票\" Style=\"font-family: \\005FAE\\008F6F\\0096C5\\009ED1; font-size: 9.75pt; font-weight: normal; vertical-align: middle; ddo-char-set: 134\" /><Control Type=\"AR.Field\" Name=\"TextBox3\" DataField=\"totalnum\" Left=\"1700.787\" Top=\"2061.355\" Width=\"425.1969\" Height=\"323.1494\" Text=\"100\" Style=\"font-family: \\005B8B\\004F53; font-size: 9.75pt; font-weight: bold; vertical-align: middle; ddo-char-set: 134\" /><Control Type=\"AR.Field\" Name=\"TextBox4\" DataField=\"ctime\" Left=\"1692.85\" Top=\"1133.858\" Width=\"1779.591\" Height=\"255.1181\" Text=\"2014-09-17 13:00:00\" OutputFormat=\"yyyy-MM-dd HH:mm\" Multiline=\"0\" Style=\"font-family: \\005FAE\\008F6F\\0096C5\\009ED1; font-size: 9.75pt; font-weight: normal; vertical-align: middle; white-space: nowrap; ddo-char-set: 134; ddo-wrap-mode: nowrap\" /><Control Type=\"AR.Label\" Name=\"Label8\" Left=\"560.1263\" Top=\"2396.409\" Width=\"1062.992\" Height=\"225.6385\" Caption=\"温馨提示：\" Style=\"font-family: \\0096B6\\004E66; font-size: 9pt; font-weight: bold; ddo-char-set: 134\" /><Control Type=\"AR.Label\" Name=\"Label10\" Left=\"560.1263\" Top=\"1717.796\" Width=\"1083.402\" Height=\"323.1492\" Caption=\"票      号：\" Style=\"font-family: \\005FAE\\008F6F\\0096C5\\009ED1; font-size: 11.25pt; text-align: justify; vertical-align: top; white-space: nowrap; ddo-char-set: 134; ddo-wrap-mode: nowrap\" Multiline=\"0\" /><Control Type=\"AR.Field\" Name=\"TextBox9\" DataField=\"ticketcode\" Left=\"2465.575\" Top=\"1686.615\" Width=\"1949.102\" Height=\"351.496\" Text=\"ZZ20180111000001587\" OutputFormat=\"\" Multiline=\"0\" Style=\"font-family: \\005FAE\\008F6F\\0096C5\\009ED1; font-size: 9.75pt; font-weight: normal; text-align: right; vertical-align: middle; white-space: nowrap; ddo-char-set: 134; ddo-wrap-mode: nowrap\" /><Control Type=\"AR.Label\" Name=\"Label11\" Left=\"1533.543\" Top=\"2396.409\" Width=\"3923.149\" Height=\"778.3936\" Caption=\"1、取票后请到入口安检处进行安检，并扫描二维码后入馆参观。&#xD;&#xA;2、参观券入馆时间为1小时，取票后请及时入馆，参观券遗失不补，请妥善不管。&#xD;&#xA;3、请照看好老人和小孩，有序入馆，文明参观。&#xD;&#xA;4、参观券最终解释权归属滁州博物馆。\" Style=\"font-family: \\005B8B\\004F53; font-size: 6.75pt; font-weight: bold; ddo-char-set: 134\" /><Control Type=\"AR.Field\" Name=\"TextBox5\" Left=\"1698.52\" Top=\"1434.331\" Width=\"1773.921\" Height=\"255.1181\" Text=\"2014-09-17 16:00:00\" OutputFormat=\"yyyy-MM-dd HH:mm\" Multiline=\"0\" Style=\"font-family: \\005FAE\\008F6F\\0096C5\\009ED1; font-size: 9.75pt; font-weight: normal; vertical-align: middle; white-space: nowrap; ddo-char-set: 134; ddo-wrap-mode: nowrap\" /><Control Type=\"AR.Image\" Name=\"Picture1\" Left=\"1844.22\" Top=\"212.5984\" Width=\"1648.063\" Height=\"425.1968\" LineWeight=\"0\" SizeMode=\"2\">0D0D000402B408000089504E470D0A1A0A0000000D494844520000011A0000003408060000005562F748000000017352474200AECE1CE90000000467414D410000B18F0BFC61050000085E49444154785EED5DED921C3908DBF77FE9A436C94CF57A0C92007F74C7F975754D6321814C6F2EB9AF5F9BFCFAFAFADA04C9817118380C5433B0CD747F1BCD319B6A794FBEC3C01E0C1CA3D9438783E230F06806B6309AD73673369AFBF6DAD948EFABDD0CE45B19CD8C82CF1963183897C5185E9F92B5D468221BC969D0A7B4D2AF3F3F636B7BE06C3AFBE8BB520BD368145051B388BEB78B740A4757CC1143DEA5660FC7554FEF9FEF50CBD330B67ACCAE2F6D349902A2833A9BA4DE7919937CA2D1B026F3C4DA77E8470B434F971578534693292233A82B886ACFCCE0DFD160A3981483899EB183DE77C46069B3A296AED1300DF13F9BCCB7501546B3CBED1ED94A91C1585BE08A264767EEA203C2A93E8FE8AA9EC1C6878C26FBFDCD18195BC08AB80A0177E240AD2762322B7462CFDC490B16338A8B2C029177108ED773D368D0CF255A7158B1329B005BD4E8386B30590EB21B51657D6A7379A6A4E6AAAC239B4BD12E7BD6E8F7233A6497075493F4DBDB6C9379873ECD68AEB52ACDBA0B0FCA36E3617EC296F3844FA888C9BCB78E7FFF7982D213C860DC8D46DD667A40510E16E06E718CD93298EF66343DBCC85C46342CC36D3486319A5D7443F315C599312A77C16045610158B77AAFE9D89C2CC61971554673FD7C9A819B694C84E33A88AAC928DB1EC2B1FA79748847E2B6F4889E59AD17FDE9C49A822542A431A3248D7CCF329A48F3558BA9D61DD938181D7B9F93AB6B55B9B9D3E7BF6732CC9656C98D958B369ACCE711D39C28660619CC19C868981C2D97CA3B95B1AA397A1A791B9A7ACECA1AD9B377314E3437A370B28BC7BBD75B629103AA85B1F16C1CDB08D1383414686353CE1DD5042C06546BAF37A23AADA835B2B1A9DCB1F1A3E2A27A44F0A0B3DC2DB0B7E62A6B2372366FADF3566C747B468842EFA8B5A8DC79838BB08D78DE331ACF7C50A37983FD24A3417D3242AB5E4E458F0C26F69CA146C39A52F5ED99214E11CD33C3ACD17CBFBF6200DFEBACF1DB99686B8B34DEEC3AD96D066DF0D15EA9EE4F75BBBCF656A4C6F6FD9E7ECA4CFFF8190DBACD14F06C337A0D3873AB616E29E5C66645983D808AC9B40D6AD5CF6CADB3EB648C268A69665F2A46D7D6C370C0E447318CA95246732D801920D56418C19918A6602B06190D6332D7C164451E5D176A12C6242C73B2B63DF48938A366867FA697DBDA19A3CEF421F32EAB19EA59B425A15961B0BEF9629AA537400844C46C14E02362BDC663EB61F8ECC58CA8C7C292B991191E561B0D32197650199D22B9325A2BE77997E775A6BD2F95AA4B016E34D63A768CE6EFDF26879AFA2522E22BD37CE8DD883920BC2867EFE6F71A1AD5A03C47D894616531337DA0D4D08B5571B7D8DB85C1D288AD59A9A7D468A20257B9A652B812ABD4A5E41D212832885E73B5B79B57436FA02CC345B12A574CBCA215DB77DEA66B6D8D0C563586BDD82C4C2F9D511E961705BF6B34BD032D108ADB2AB14A31A362D16DC536226B02A3EA68F346705B5CB0B94634B137589EF1303C23EDD15033673031C81CAE9716323FAF264FC78C7692D1B0005533CA14C088948D619A2D5343E6DD4C6DAC39A06162F899B5BD555C62ECCDDFE33EC229D290A9A9A781858599636BF37D7183307F5C6A5EC2362903B01D1AEF4619218A4A008A578708E5F39A33F26EE69D08FF68A341C3C7F2595117CA61F5776B8A0AE608A70C4E65F6105ED680AEB39CADEBBDD1445D33BAB6223210F9B39E3338D3222CFADF014771B797C9F5B24243DAEBB3595A7EDCB2E087F92D3F56DDE85325535FC55C7A75A36DB56A132D351AD4644C51195146BCCB0C2363461E36A68157D5C69ECB6EAED72D6255DDBD3E459BD8F51D46CB765B627954CD306204DE45D1EB776606507DAED1A0A211E1E8B957404571A878F45C31900CDECCBBA8064683680EC65CAEDB4ED5F055E26572BD70333A3131CC992D6F95B362E960F57B454DC38C8621D31B6465C899B32231B330540899ADCF7ABFB7817806E3E559653411BCCC86138DC96835F24C646611DC6FC36C572FB4CE2A43A1E46A9B70D690A3C1403564C8EFDD5A15F9D81C484B666361F961E358EC4C1C839FC9C3EAB4AA67918E6C8D5579BA6618351A049E01AD6C34B36FC3994DC37085F88E3C47E7329B00CAD10E6904A7FA0E83FBDAF76C7E74E3CFEC992B665603542793277A61FCF974EA098340A1E70A680BFC0EC2458945FC8C6814E5CC56F7EE2DD4FCAE0C13635D083378547B59C5C46C484CDFAB3AA178B50E2B9F356FAC717B3887180D6B10280E3D4702449FCF3E7745733246A3F08738AB1A068449E152C5A4180DC259F91C5DD4EC594A7D3277EA2D80402BDB116ACEEA6140D8ABB960CE5B51636F9B62B1BAB7D665036AE3D4C68CE2617AAAA7333AAF6760D660A25CD5CF9141B0E7A13CCA6C7FE8FFFA17AC406CA331F9981896A4AAB81598945BB8AACE56F78ABC5E1DB38CC6BA2CD010A1FAADDA56F44BEFA2A8303D2587AAA7F9DBDB88F8DE7324660B6EB548568DB3075F152DA20DAAB53227EA8DD167A1F32337F34A8DA297FB3766E597C28BCAC7C71FAA54802141D5DB257BF61DDF9F6D6A1F2BEDC43FFEA036E72E7AAED608F1A01A0ABA74466C6F9AE5A18A8DE7EAA6133CE696AFAD1EBE99E7CF3CABB219EE8A5BE5809DD388B14D311AF69B5225E6C4E71998394433CFCA33F337C3EEDB4C559DA3BF3EA61B8D653A95849D5C1A03A11B4AFCFEBF36B2866E6DF4311AEEAFAB452A2D351A04EE3C9FC34074D3500DEA6E437B37BC15DD823E9FA2671CA38932F7A0F7A246C36C2956E3DE81BEFFD16846E9728C6614B337CA9B311AF5DBFE2EC37B179C7769B3633477516A20CE8AA1422B77FB7C603925A92B382901F29024C7681E2264A68C2A13A8CA93A9A5E2DD6332152CFECC718CA69ED393F1A60CF4B6B29B96B21DEC6334DB497200AD62E0291BD92AFEBC738FD1ECA8CAC17418781803C7681E26E829E730B02303BF0194CEC22BB72A08C70000000049454E44AE426082</Control><Control Type=\"AR.Image\" Name=\"Picture2\" Left=\"496.063\" Top=\"212.5984\" Width=\"1311.874\" Height=\"477.3544\" LineWeight=\"0\">0D0D0004023E02000089504E470D0A1A0A0000000D4948445200000051000000310806000000A27B456C000000017352474200AECE1CE90000000467414D410000B18F0BFC6105000001E8494441546843ED9AE16E83300C8479FF9766635A359612FBCE3E606857A9BFE2D8F1974B6C6897D59F3681A5EDC10E56431488C0100D514040E0C24A34440101810B2BD1100504042E2E55E2B22CEB9D5F01AF431721C42D61E5E74E80EA5CF65C524ACAE04F8738639142DC88BF92EFAAF2C9102306104415C82388DD8D99CD57C6CA440443DC83AC1E716562197C45ACBD8F281E05B10B52915806EF35DE8D8502FC62822EEA57351A5A15D447373134CEB8D9D971ECE65682585DE45F8738AE0FDDB4324416E4AC32A30B65ED9878DDCD6D419C818C0A4F75B7BB108FE633A0A58585590C7317B1903AF651BF5AF1DB5662540DAF521D9A78D6ECA37E463B19440666B5CFAC249981539C1639C4E89EEC5EE00C44149E62434F81C8A832EAD11810230C642EB329A717963040F20E5105F1ED9E22E276619EAA44041062535195AA7D41005F061169CE993B33ABFC8C2F04D4ADC739531A331ED922C7B90B6B36DF4A1490354443FCF9F5D0C7F99B00530C5C58763D9C0B0B799F644ACBC65D9D3F096490B271433444F2DC4ECC33A565E356A2956825FE8B6767E47DA2460AEF5E6E7FECCB5E734D1FFA0BFF7534C48140069F29525DB8566297E05630053E60174AF5287DC109CC5AB7AE0366BE3271D617B34ED6F65225B28B7B8ABD210A76CA1005103F0023C730ABD0724E7A0000000049454E44AE426082</Control><Control Type=\"AR.Label\" Name=\"Label2\" Left=\"1700.787\" Top=\"1686.615\" Width=\"1440\" Height=\"354.3307\" Caption=\"CZBWG\" Style=\"font-family: \\005FAE\\008F6F\\0096C5\\009ED1; font-size: 9.75pt; vertical-align: middle; ddo-char-set: 0\" /><Control Type=\"AR.Image\" Name=\"Picture3\" Left=\"4074.52\" Top=\"1806.803\" Width=\"319.181\" Height=\"274.3937\" BackColor=\"16777215\" BackStyle=\"1\" LineWeight=\"0\" /></Section></Sections><ReportComponentTray /><Script><![CDATA[public void Detail_BeforePrint()\r\n{\r\n\tTextBox5.Text = Convert.ToDateTime(TextBox4.Text).AddHours(1).ToString(\"yyyy-MM-dd HH:mm:ss\");\r\n}\r\n]]></Script><PageSettings LeftMargin=\"0\" RightMargin=\"0\" TopMargin=\"0\" BottomMargin=\"0\" Orientation=\"2\" /><Parameters /></ActiveReportsLayout>";
            SectionReport rpt = ReportLayout(template, ref errorMessage);

            string exportPdfPath = ReportExportToPdf(rpt, printWidth, printHeight, ref errorMessage);

            string imageOutputPath = physicalApplicationPath + "ImageTemplate\\";
            string imageName = "T" + DateTime.Now.ToString("MMddHHmmssfff");
            string imageCovertPath = ConvertPDF2Image(exportPdfPath, imageOutputPath, imageName, 1, 1, ImageFormat.Png, Definition.Ten, ref errorMessage);

            string imageSavePath = physicalApplicationPath + "ImageTemplate\\new" + imageName + ".PNG";
            Image image = Image.FromFile(imageCovertPath);
            CutEllipse(image, new Rectangle(0, 0, printWidth * 2, printHeight * 2), new Size(printWidth, printHeight), imageSavePath, ref errorMessage);
            image.Dispose();

            imageBytes = ImageToBytes(imageSavePath, ref errorMessage);
            return imageBytes;
        }

        public SectionReport ReportLayout(string template, ref string errorMessage)
        {
            GrapeCity.ActiveReports.SectionReport rpt = null;
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(template)))
            {
                using (XmlReader xr = XmlReader.Create(ms))
                {
                    List<dynamic> DataSource = new List<dynamic>();
                    DataSource.Add(new
                    {
                        TicketID = "ticketId",
                        CardMoney = "cardMoney",
                        TdCode = "tdCode",
                        TicketCode = "ticketCode",
                        TicketTypeName = "ticketTypeName",
                        MemberName = "memberName",
                        TicPrice = "ticketPrice",
                        ReaPrice = "reaPrice",
                        PrintPrice = "printPrice",
                        TicMoney = "ticMoney",
                        ReaMoney = "reaMoney",
                        PrintMoney = "printMoney",
                        STime = "2020-04-05",
                        ETime = "2020-04-04",
                        SDate = "2020-04-03",
                        CDate = "2020-04-02",
                        CTime = "2020-04-02 11:12:13",
                        CertNo = "certNo",
                        PersonNum = "personNum",
                        TotalNum = "totalNum",
                        TicketNum = "ticketNum",
                        SalePointName = "salePointName",
                        ParkName = "parkName",
                        BuyerName = "buyerName",
                        CertTypeName = "certTypeName",
                        场次 = "memo"
                    });
                    rpt = new SectionReport();
                    rpt.LoadLayout(xr);
                    rpt.DataSource = DataSource;

                    rpt.Run();
                }
            }
            if (rpt == null && string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "票版加载失败";
            }
            return rpt;
        }

        public string ReportExportToPdf(SectionReport rpt, int printWidth, int printHeight, ref string errorMessage)
        {
            string exportPdfPath = "";
            if (rpt != null && rpt.Document.Pages.Count > 0 && rpt.Document.Pages[0].Width < printWidth && rpt.Document.Pages[0].Height < printHeight)
            {
                GrapeCity.ActiveReports.Export.Pdf.Section.PdfExport pdfExport1 = new GrapeCity.ActiveReports.Export.Pdf.Section.PdfExport();
                string pdfOutPath = physicalApplicationPath + "PdfTemplate\\";
                if (Directory.Exists(pdfOutPath))
                {
                    Directory.Delete(pdfOutPath, true);
                }
                Directory.CreateDirectory(pdfOutPath);
                string pdfName = "T" + DateTime.Now.ToString("MMddHHmmssfff") + ".pdf";
                pdfExport1.Export(rpt.Document, pdfOutPath + pdfName, "1-2");
                pdfExport1.Dispose();
                exportPdfPath = pdfOutPath + pdfName;
            }
            rpt.Dispose();
            if (string.IsNullOrEmpty(exportPdfPath) && string.IsNullOrEmpty(errorMessage))
            {
                errorMessage += "导出Pdf失败";
            }
            return exportPdfPath;
        }

        public enum Definition
        {
            One = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10
        }

        /// <summary>
        /// 将PDF文档转换为图片的方法
        /// </summary>
        /// <param name="pdfInputPath">PDF文件路径</param>
        /// <param name="imageOutputPath">图片输出路径</param>
        /// <param name="imageName">生成图片的名字</param>
        /// <param name="startPageNum">从PDF文档的第几页开始转换</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换</param>
        /// <param name="imageFormat">设置所需图片格式</param>
        /// <param name="definition">设置图片的清晰度，数字越大越清晰</param>
        public static string ConvertPDF2Image(string pdfInputPath, string imageOutputPath,
            string imageName, int startPageNum, int endPageNum, ImageFormat imageFormat, Definition definition, ref string errorMessage)
        {
            string imageCovertPath = imageOutputPath + imageName + "1.PNG";
            try
            {
                if (Directory.Exists(imageOutputPath))
                {
                    Directory.Delete(imageOutputPath, true);
                }
                Directory.CreateDirectory(imageOutputPath);
                PDFFile pdfFile = PDFFile.Open(pdfInputPath);
                // validate pageNum
                if (startPageNum <= 0)
                {
                    startPageNum = 1;
                }
                if (endPageNum > pdfFile.PageCount)
                {
                    endPageNum = pdfFile.PageCount;
                }
                if (startPageNum > endPageNum)
                {
                    int tempPageNum = startPageNum;
                    startPageNum = endPageNum;
                    endPageNum = startPageNum;
                }
                // start to convert each page
                for (int i = startPageNum; i <= endPageNum; i++)
                {
                    Bitmap pageImage = pdfFile.GetPageImage(i - 1, 56 * (int)definition);
                    pageImage.Save(imageOutputPath + imageName + i.ToString() + "." + imageFormat.ToString(), imageFormat);
                    pageImage.Dispose();
                }
                pdfFile.Dispose();
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = ex.Message;
                }
            }

            if (!System.IO.File.Exists(imageCovertPath) && string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "PDF转图片失败";
            }
            return imageCovertPath;
        }

        private Image CutEllipse(Image image, Rectangle rec, Size size, string imageSavePath, ref string errorMessage)
        {
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                using (TextureBrush br = new TextureBrush(image, System.Drawing.Drawing2D.WrapMode.Clamp, rec))
                {
                    br.ScaleTransform(bitmap.Width / (float)rec.Width, bitmap.Height / (float)rec.Height);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.FillRectangle(br, new Rectangle(Point.Empty, size));
                    g.Dispose();
                    br.Dispose();
                    image.Dispose();
                }
            }
            bitmap.Save(imageSavePath, System.Drawing.Imaging.ImageFormat.Png);
            bitmap.Dispose();
            if (!System.IO.File.Exists(imageSavePath) && string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "图片裁剪失败";
            }
            return null;
        }

        public byte[] ImageToBytes(string imagePath, ref string errorMessage)
        {
            byte[] imageBytes = null;
            using (FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                imageBytes = new byte[fileStream.Length];
                fileStream.Read(imageBytes, 0, imageBytes.Length);
                fileStream.Dispose();
            }
            if (imageBytes == null && string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = "图片转字符数组失败";
            }
            return imageBytes;
        }



        public void GetTemplateImage(string fileName, DirectoryInfo outputDirectory)
        {
            // Create an output directory.
            string qrCodeUrl = _qrCodeHelper.CreateQrCode(HttpContext.Current.Request.PhysicalApplicationPath, "JN12341923481234", "htpps://www.baidu.com");
            PageReport pageReport = null;
            //GrapeCity.ActiveReports.PageReport report = new GrapeCity.ActiveReports.PageReport(ActiveReportsHelper.GetReport("Template.TicketTemplate.rdlx"));
            string rdlxStr = File.ReadAllText("C:\\code\\TicketTemplate.rdlx");
            
            using (MemoryStream memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(rdlxStr)))
            {
                StreamReader streamReader = new StreamReader(memoryStream);
                pageReport = new PageReport(streamReader);

                GetDataTable();

                GrapeCity.ActiveReports.Document.PageDocument reportDocument = new GrapeCity.ActiveReports.Document.PageDocument(pageReport);
                reportDocument.LocateDataSource += Document_LocateDataSource;
                //reportDocument.Print(false, false, false);

                // Provide settings for your rendering output.
                GrapeCity.ActiveReports.Export.Image.Page.Settings imageSetting = new GrapeCity.ActiveReports.Export.Image.Page.Settings();
                imageSetting.ImageType = GrapeCity.ActiveReports.Export.Image.Page.Renderers.ImageType.PNG;
                imageSetting.PageWidth = "32cm";
                imageSetting.PageHeight = "16cm";
                imageSetting.EndPage = 0;
                imageSetting.Pagination = false;
                GrapeCity.ActiveReports.Extensibility.Rendering.ISettings setting = imageSetting;

                // Set the rendering extension and render the report.
                GrapeCity.ActiveReports.Export.Image.Page.ImageRenderingExtension imageRenderingExtension = new GrapeCity.ActiveReports.Export.Image.Page.ImageRenderingExtension();
                GrapeCity.ActiveReports.Rendering.IO.FileStreamProvider outputProvider = new GrapeCity.ActiveReports.Rendering.IO.FileStreamProvider(outputDirectory, System.IO.Path.GetFileNameWithoutExtension(fileName));

                // Overwrite output file if it already exists.
                outputProvider.OverwriteOutputFile = true;

                reportDocument.Render(imageRenderingExtension, outputProvider, imageSetting);

                Designer designer = new Designer();
            }
        }

        private byte[] GetImageByte()
        {
            string fileName = "T" + DateTime.Now.ToString("MMddHHmmssfff");
            System.IO.DirectoryInfo outputDirectory = new System.IO.DirectoryInfo(HttpContext.Current.Request.PhysicalApplicationPath + @"\TicketTemplateImage");
            outputDirectory.Create();
            GetTemplateImage(fileName, outputDirectory);
            fileName = outputDirectory.FullName + "\\" + fileName + "001.PNG";
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] imageBytes = new byte[fileStream.Length];
                fileStream.Read(imageBytes, 0, imageBytes.Length);
                return imageBytes;
            }
        }

        private void GetDataTable()
        {
            ticketDataTable.Columns.Add("CompanyName");
            ticketDataTable.Columns.Add("TicketTypeName");
            ticketDataTable.Columns.Add("ETime");
            ticketDataTable.Columns.Add("PersonNum");
            ticketDataTable.Columns.Add("TicketCode");
            ticketDataTable.Columns.Add("CTime");
            ticketDataTable.Columns.Add("DistributorName");
            ticketDataTable.Columns.Add("ReaMoney");
            ticketDataTable.Columns.Add("SalePointName");
            ticketDataTable.Columns.Add("ChangCi");
            ticketDataTable.Columns.Add("Seat");
            DataRow dataRow = ticketDataTable.NewRow();
            dataRow["CompanyName"] = ticketTemplatePrintInput.CompanyName;
            dataRow["TicketTypeName"] = ticketTemplatePrintInput.TicketTypeName;
            dataRow["ETime"] = ticketTemplatePrintInput.ETime;
            dataRow["PersonNum"] = ticketTemplatePrintInput.PersonNum;
            dataRow["TicketCode"] = ticketTemplatePrintInput.TicketCode;
            dataRow["CTime"] = ticketTemplatePrintInput.CTime;
            dataRow["DistributorName"] = ticketTemplatePrintInput.DistributorName;
            dataRow["ReaMoney"] = ticketTemplatePrintInput.ReaMoney;
            dataRow["SalePointName"] = ticketTemplatePrintInput.SalePointName;
            dataRow["ChangCi"] = ticketTemplatePrintInput.ChangCi;
            dataRow["Seat"] = ticketTemplatePrintInput.Seat;
            ticketDataTable.Rows.Add(dataRow);
        }

        private void Document_LocateDataSource(object sender, LocateDataSourceEventArgs args)
        {
            args.Data = ticketDataTable;
        }
    }
}