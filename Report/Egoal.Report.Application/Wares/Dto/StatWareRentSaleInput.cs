using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egoal.Report.Wares.Dto
{
    public class StatWareRentSaleInput
    {
        public DateTime? SCTime { get; set; }

        public DateTime? ECTime { get; set; }

        public string WareName { get; set; }

        public string ListNo { get; set; }

        public int? WareTypeTypeId { get; set; }

        public string WareTypeTypeName { get; set; }

        public int? WareTypeId { get; set; }

        public string WareTypeName { get; set; }

        public int? MerchantId { get; set; }

        public string MerchantName { get; set; }

        public int? ShopTypeId { get; set; }

        public string ShopTypeName { get; set; }

        public int? WareShopId { get; set; }

        public string WareShopName { get; set; }

        public Guid? SupplierId { get; set; }

        public string SupplierName { get; set; }

        public int? CashierId { get; set; }

        public string CashierName { get; set; }

        public int? CashPcId { get; set; }

        public string CashPcName { get; set; }




        public string CenterServerFlag { get; set; }
    }
}
