using Egoal.Application.Services.Dto;
using Egoal.Common.Dto;
using System.Collections.Generic;

namespace Egoal.Scenics.Dto
{
    public class GroundChangCisDto
    {
        public GroundChangCisDto()
        {
            ChangCis = new List<ComboboxItemDto<int>>();
            ChangCiDtos = new List<ChangCiDto>();
        }

        public int GroundId { get; set; }
        public string GroundName { get; set; }
        public bool HasGroundSeat { get; set; }
        public bool HasGroundChangCi { get; set; }
        public List<ComboboxItemDto<int>> ChangCis { get; set; }
        public List<ChangCiDto> ChangCiDtos { get; set; }
    }
}
