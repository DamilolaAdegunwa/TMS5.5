using Egoal.Extensions;
using System.Xml.Linq;

namespace Egoal.Payment.ABCPay
{
    public class NotifyRequest
    {
        public string ReturnCode { get; set; }
        public string ErrorMessage { get; set; }
        public string OrderNo { get; set; }
        public string Amount { get; set; }
        public string BatchNo { get; set; }
        public string VoucherNo { get; set; }
        public string HostDate { get; set; }
        public string HostTime { get; set; }
        public string MerchantRemarks { get; set; }
        public string PayType { get; set; }
        public string NotifyType { get; set; }
        public string iRspRef { get; set; }
        public string bank_type { get; set; }
        public string ThirdOrderNo { get; set; }

        public static NotifyRequest FromXml(XElement xElement)
        {
            NotifyRequest notifyRequest = new NotifyRequest();
            notifyRequest.ReturnCode = xElement.Element("ReturnCode")?.Value;
            notifyRequest.ErrorMessage = xElement.Element("ErrorMessage")?.Value;
            notifyRequest.OrderNo = xElement.Element("OrderNo")?.Value;
            notifyRequest.Amount = xElement.Element("Amount")?.Value;
            notifyRequest.BatchNo = xElement.Element("BatchNo")?.Value;
            notifyRequest.VoucherNo = xElement.Element("VoucherNo")?.Value;
            notifyRequest.HostDate = xElement.Element("HostDate")?.Value;
            notifyRequest.HostTime = xElement.Element("HostTime")?.Value;
            notifyRequest.MerchantRemarks = xElement.Element("MerchantRemarks")?.Value;
            notifyRequest.PayType = xElement.Element("PayType")?.Value;
            notifyRequest.NotifyType = xElement.Element("NotifyType")?.Value;
            notifyRequest.iRspRef = xElement.Element("iRspRef")?.Value;
            notifyRequest.bank_type = xElement.Element("bank_type")?.Value;
            notifyRequest.ThirdOrderNo = xElement.Element("ThirdOrderNo")?.Value;

            return notifyRequest;
        }

        public NotifyCommand ToNotifyCommand()
        {
            var command = new NotifyCommand();
            command.PaySuccess = ReturnCode == "0000";
            command.BankType = bank_type;
            command.TotalFee = Amount.To<decimal>();
            command.TransactionId = ThirdOrderNo;
            command.ListNo = OrderNo;
            command.PayTime = $"{HostDate} {HostTime}".ToDateTime("yyyy/MM/dd HH:mm:ss");

            return command;
        }
    }
}
