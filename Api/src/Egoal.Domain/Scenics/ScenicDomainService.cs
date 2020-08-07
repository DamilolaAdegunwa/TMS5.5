using Egoal.Common;
using Egoal.Domain.Services;
using System;
using System.Threading.Tasks;

namespace Egoal.Scenics
{
    public class ScenicDomainService : DomainService, IScenicDomainService
    {
        private readonly IGroundDateChangCiSaleNumRepository _groundDateChangCiSaleNumRepository;

        public ScenicDomainService(
            IGroundDateChangCiSaleNumRepository groundDateChangCiSaleNumRepository)
        {
            _groundDateChangCiSaleNumRepository = groundDateChangCiSaleNumRepository;
        }

        public async Task<int> GetGroundSeatSurplusQuantityAsync(Ground ground, DateTime date, ChangCi changCi)
        {
            var saleQuantity = await _groundDateChangCiSaleNumRepository.GetSaleQuantityAsync(ground.Id, date, changCi.Id);

            if (changCi.ChangCiNum.HasValue && changCi.ChangCiNum.Value > 0)
            {
                return changCi.ChangCiNum.Value - changCi.ReservedNum - saleQuantity;
            }

            return (ground.SeatNum ?? 0) - saleQuantity;
        }
    }
}
