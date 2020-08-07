using Egoal.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Payment.ABCPay
{
    public class QueryOrderResponse
    {
        public string ReturnCode { get; set; }
        public string ErrorMessage { get; set; }
        public string TrxType { get; set; }
        public string Order { get; set; }

        public OrderInfo OrderObj { get; set; }

        public class OrderInfo
        {
            public string PayTypeID { get; set; }
            public string OrderNo { get; set; }
            public string OrderDate { get; set; }
            public string OrderTime { get; set; }
            public string OrderAmount { get; set; }
            public string Status { get; set; }
            public string OrderDesc { get; set; }
            public string OrderURL { get; set; }
            public string PaymentLinkType { get; set; }
            public string AcctNo { get; set; }
            public string CommodityType { get; set; }
            public string ReceiverAddress { get; set; }
            public string BuyIP { get; set; }
            public string iRspRef { get; set; }
            public string ReceiveAccount { get; set; }
            public string ReceiveAccName { get; set; }
            public string MerchantRemarks { get; set; }
            public string UserID { get; set; }
            public string ThirdOrderNo { get; set; }
            public string szAccDate { get; set; }
            public string BankType { get; set; }
            public string BUYER_USER_ID { get; set; }
            public string RefundAmount { get; set; }
            public string MerRefundAccountNo { get; set; }
            public string MerRefundAccountName { get; set; }
            public List<OrderItem> OrderItems { get; set; }
        }

        public class OrderItem
        {
            public string SubMerName { get; set; }
            public string SubMerId { get; set; }
            public string SubMerMCC { get; set; }
            public string SubMerchantRemarks { get; set; }
            public string ProductID { get; set; }
            public string ProductName { get; set; }
            public string UnitPrice { get; set; }
            public string Qty { get; set; }
            public string ProductRemarks { get; set; }
        }

        public QueryPayResult ToQueryPayResult()
        {
            var output = new QueryPayResult();
            output.ErrorMessage = ErrorMessage;
            output.IsExist = !ErrorMessage.Contains("订单不存在");

            if (!Order.IsNullOrEmpty())
            {
                DecodeOrder();

                output.OpenId = OrderObj.BUYER_USER_ID;
                output.BankType = OrderObj.BankType;
                output.TotalFee = OrderObj.OrderAmount.To<decimal>();
                output.TransactionId = OrderObj.ThirdOrderNo;
                output.ListNo = OrderObj.OrderNo;
                output.PayTime = OrderObj.szAccDate.IsNullOrEmpty() ? DateTime.Now : OrderObj.szAccDate.ToDateTime(ABCPayOptions.DateTimeFormat);
                output.IsPaid = OrderObj.Status == "03" || OrderObj.Status == "04";
                output.IsPaying = OrderObj.Status == "01" || OrderObj.Status == "02";
                output.IsRefund = OrderObj.Status == "05";
            }

            return output;
        }

        public QueryRefundResult ToQueryRefundResult()
        {
            var result = new QueryRefundResult();

            if (!Order.IsNullOrEmpty())
            {
                DecodeOrder();

                result.ListNo = OrderObj.OrderNo;
                result.RefundListNo = OrderObj.iRspRef;
                if (!OrderObj.RefundAmount.IsNullOrEmpty())
                {
                    result.RefundFee = OrderObj.RefundAmount.To<decimal>();
                }
                result.Success = OrderObj.Status == "05";
            }

            result.RefundTime = DateTime.Now;
            result.ErrorMessage = ErrorMessage;
            result.ShouldRetry = ReturnCode != "0000";
            result.IsExist = !ErrorMessage.Contains("订单不存在");

            return result;
        }

        private void DecodeOrder()
        {
            OrderObj = Encoding.GetEncoding("gb2312").GetString(Convert.FromBase64String(Order)).JsonToObject<OrderInfo>();
        }
    }
}
