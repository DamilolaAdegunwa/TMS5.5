namespace Egoal.Orders
{
    public class OrderOptions
    {
        /// <summary>
        /// 团队订单每单最大数量
        /// </summary>
        public int GroupOrderMaxQuantity { get; set; }

        /// <summary>
        /// 散客订单每单最大成人数量
        /// </summary>
        public int IndividualOrderMaxAdultQuantity { get; set; }

        /// <summary>
        /// 散客订单每单最大儿童数量
        /// </summary>
        public int IndividualOrderMaxChildrenQuantity { get; set; }

        /// <summary>
        /// 每个成人最多携带几个儿童
        /// </summary>
        public int PerAdultMaxChildrenQuantity { get; set; }

        /// <summary>
        /// 证件购票周期(配合“证件可购次数”参数使用,0为不限)
        /// </summary>
        public int CertTicketSaleDaysRange { get; set; }

        /// <summary>
        /// 证件可购次数(周期内可购次数,0为不限)
        /// </summary>
        public int CertTicketSaleNum { get; set; }
    }
}
