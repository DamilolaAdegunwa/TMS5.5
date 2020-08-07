using Egoal.Application.Services;
using Egoal.Caches;
using Egoal.Domain.Repositories;
using Egoal.Extensions;
using Egoal.Runtime.Session;
using Egoal.Tickets.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Egoal.Tickets
{
    public class PrintTicketAppService : ApplicationService
    {
        private readonly ISession _session;
        private readonly INameCacheService _nameCacheService;
        private readonly ITicketSaleRepository _ticketSaleRepository;
        private readonly IRepository<TicketReprintLog, long> _ticketReprintLogRepository;

        public PrintTicketAppService(
            ISession session,
            INameCacheService nameCacheService,
            ITicketSaleRepository ticketSaleRepository,
            IRepository<TicketReprintLog, long> ticketReprintLogRepository)
        {
            _session = session;
            _nameCacheService = nameCacheService;
            _ticketSaleRepository = ticketSaleRepository;
            _ticketReprintLogRepository = ticketReprintLogRepository;

        }

        public async Task RePrintAsync(PrintTicketInput input)
        {
            await PrintAsync(input, true);
        }

        public async Task PrintAsync(PrintTicketInput input, bool isReprint = false)
        {
            var ticketSales = await _ticketSaleRepository.GetAll()
                .WhereIf(!input.ListNo.IsNullOrEmpty(), t => t.ListNo == input.ListNo)
                .WhereIf(!input.TicketCode.IsNullOrEmpty(), t => t.TicketCode == input.TicketCode)
                .Where(t => t.TicketStatusId != TicketStatus.已退)
                .ToListAsync();
            foreach (var ticketSale in ticketSales)
            {
                ticketSale.Print();

                if (!isReprint) continue;

                var rePrintLog = new TicketReprintLog();
                rePrintLog.TicketId = ticketSale.Id;
                rePrintLog.TicketTypeId = ticketSale.TicketTypeId;
                rePrintLog.TicketTypeName = ticketSale.TicketTypeName;
                rePrintLog.TicketCode = ticketSale.TicketCode;
                rePrintLog.CardNo = ticketSale.CardNo;
                rePrintLog.CashierId = _session.StaffId;
                rePrintLog.CashierName = _nameCacheService.GetStaffName(rePrintLog.CashierId);
                rePrintLog.CashPcid = _session.PcId;
                rePrintLog.CashPcname = _nameCacheService.GetPcName(rePrintLog.CashPcid);
                rePrintLog.SalePointId = _session.SalePointId;
                rePrintLog.ParkId = _session.ParkId;
                rePrintLog.ParkName = _nameCacheService.GetParkName(rePrintLog.ParkId);

                await _ticketReprintLogRepository.InsertAsync(rePrintLog);
            }
        }
    }
}
