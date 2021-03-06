using Egoal.Extensions;
using System.Xml.Linq;

namespace Egoal.Invoice.GuangDongBaiWangJiuBin
{
    public class InvoiceOutput
    {
        /// <summary>
        /// 发票请求流水号（业务订单号）
        /// </summary>
        public string FPQQLSH { get; set; }

        /// <summary>
        /// 税控设备编号
        /// </summary>
        public string JQBH { get; set; }

        /// <summary>
        /// 发票代码
        /// </summary>
        public string FP_DM { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        public string FP_HM { get; set; }

        /// <summary>
        /// 开票日期（yyyyMMddHHmmss）
        /// </summary>
        public string KPRQ { get; set; }

        /// <summary>
        /// 发票密文
        /// </summary>
        public string FP_MW { get; set; }

        /// <summary>
        /// 校验码
        /// </summary>
        public string JYM { get; set; }

        /// <summary>
        /// 二维码此节点返回数据量较大非标准
        /// </summary>
        public string EWM { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string BZ { get; set; }

        /// <summary>
        /// 返回代码
        /// </summary>
        public string RETURNCODE { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string RETURNMSG { get; set; }

        public static InvoiceOutput FromXml(string xml)
        {
            var output = new InvoiceOutput();

            XDocument document = XDocument.Parse(xml);
            var body = document.Element("business").Element("HTJS_DZFPKJ");
            if (body != null)
            {
                var properties = output.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var value = body.Element(property.Name)?.Value;
                    property.SetValue(output, value);
                }
            }
            else
            {
                body = document.Element("business").Element("body");
                output.RETURNCODE = body.Element("returncode").Value;
                output.RETURNMSG = body.Element("returnmsg").Value;
            }

            return output;
        }

        public InvoiceResponse ToResponse()
        {
            InvoiceResponse response = new InvoiceResponse();
            response.FPQQLSH = FPQQLSH;
            response.FP_DM = FP_DM;
            response.FP_HM = FP_HM;
            response.JYM = JYM;
            response.KPRQ = KPRQ.ToDateTime("yyyyMMddHHmmss");

            return response;
        }
    }
}
