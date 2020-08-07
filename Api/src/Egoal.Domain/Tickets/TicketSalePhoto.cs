using Egoal.Domain.Entities;
using Egoal.TicketTypes;
using System;
using System.IO;

namespace Egoal.Tickets
{
    public class TicketSalePhoto : Entity<long>
    {
        public const string FaceDirectory = "faces";

        public string Name { get; set; }
        public TicketTypeType? TicketTypeTypeId { get; set; }
        public int? TicketTypeId { get; set; }
        public long? TicketId { get; set; }
        public Guid? TradeId { get; set; }
        public byte[] Photo { get; set; }
        public byte[] PhotoTemplate { get; set; }
        public string PhotoUrl { get; set; }
        public string Stime { get; set; }
        public string Etime { get; set; }
        public int? CashierId { get; set; }
        public int? SalePointId { get; set; }
        public FaceRegSource? RegSourceId { get; set; }
        public int? RegCashPcId { get; set; }
        public int? RegGateId { get; set; }
        public DateTime? Ctime { get; set; } = DateTime.Now;
        public int? ParkId { get; set; }
        public Guid? SyncCode { get; set; } = Guid.NewGuid();

        public virtual TicketSale TicketSale { get; set; }

        public static string GetWebSavePath(string basePath)
        {
            return Path.Combine(basePath, "wwwroot", FaceDirectory);
        }
    }
}
