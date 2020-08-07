using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Settings.Dto
{
    public class SelfHelpSettingDto
    {
        public SelfHelpDatabaseDto SelfHelpDatabaseDto { get; set; }

        public SelfHelpSystemDto SelfHelpSystemDto { get; set; }

        public SelfHelpEquipmentDto SelfHelpEquipmentDto { get; set; }
    }

    public class SelfHelpDatabaseDto
    {
        public string DbType { get; set; }

        public string DataSource { get; set; }

        public string InitialCatalog { get; set; }

        public string UserId { get; set; }

        public string Password { get; set; }
    }

    public class SelfHelpSystemDto
    {
        public int SalePointId { get; set; }

        public int LoginStaffId { get; set; }

        public string ForeServerUrl { get; set; }

        public int OrderMaxNum { get; set; }

        public bool EnableGetTicket { get; set; }

        public int MaxTicketSaleNum { get; set; }

        public string GkKeyFilePath { get; set; }
    }

    public class SelfHelpEquipmentDto
    {
        public string UtcPort { get; set; }

        public string TicketPrinter { get; set; }

        public string TicketPrinterPort { get; set; }

        public string TicketPaperType { get; set; }

        public string UtcLedId { get; set; }
    }
}
