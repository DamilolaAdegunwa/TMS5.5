using Egoal.Application.Services;
using Egoal.Domain.Repositories;
using Egoal.Extensions;
using Egoal.Notifications;
using Egoal.Orders.Dto;
using Egoal.Runtime.Session;
using Egoal.Scenics;
using Egoal.Staffs;
using Egoal.Tickets;
using Egoal.Tickets.Dto;
using Egoal.UI;
using System.Threading.Tasks;

namespace Egoal.Orders
{
    public class ConsumeOrderAppService : ApplicationService
    {
        private readonly ISession _session;
        private readonly ITicketSaleRepository _ticketSaleRepository;
        private readonly ITicketSaleDomainService _ticketSaleDomainService;
        private readonly IRealTimeNotifier _realTimeNotifier;
        private readonly IRepository<Gate> _gateRepository;

        public ConsumeOrderAppService(
            ISession session,
            ITicketSaleRepository ticketSaleRepository,
            ITicketSaleDomainService ticketSaleDomainService,
            IRealTimeNotifier realTimeNotifier,
            IRepository<Gate> gateRepository)
        {
            _session = session;
            _ticketSaleRepository = ticketSaleRepository;
            _ticketSaleDomainService = ticketSaleDomainService;
            _realTimeNotifier = realTimeNotifier;
            _gateRepository = gateRepository;
        }

        public async Task ConsumeOrderFromMobileAsync(ConsumeOrderFromMobileInput input)
        {
            var mobileGate = await _gateRepository.FirstOrDefaultAsync(g => g.GateTypeId == GateType.手机);
            if (mobileGate == null)
            {
                throw new UserFriendlyException("手机检票通道未设置");
            }

            var ticketSales = await _ticketSaleRepository.GetAllListAsync(t => t.OrderListNo == input.ListNo);
            if (ticketSales.IsNullOrEmpty())
            {
                return;
            }

            foreach (var ticketSale in ticketSales)
            {
                var consumeInput = new ConsumeTicketInput();
                consumeInput.ConsumeNum = ticketSale.PersonNum.Value;
                consumeInput.ConsumeType = ConsumeType.手机检票;
                consumeInput.GateId = mobileGate.Id;
                consumeInput.GateGroupId = mobileGate.GateGroupId;
                consumeInput.CheckerId = _session.StaffId;

                await _ticketSaleDomainService.ConsumeAsync(ticketSale, consumeInput);
            }

            await _realTimeNotifier.NoticeCheckerCheckInAsync(input);
            await _realTimeNotifier.NoticeExplainerCheckInAsync(input);
        }
    }
}
