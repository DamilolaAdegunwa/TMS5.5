using Egoal.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Orders.Dto
{
    public class SelfHelpTicketGroundOutDto
    {
        public string TicketCode { get; set; }

        public string TicketStatusName { get; set; }

        public PagedResultDto<SelfHelpTicketGroundListDto> PageResult { get; set; }
    }
}
