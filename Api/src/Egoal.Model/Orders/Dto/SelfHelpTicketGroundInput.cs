using Egoal.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Orders.Dto
{
    public class SelfHelpTicketGroundInput : PagedInputDto
    {
        public string TicketCode { get; set; }
    }
}
