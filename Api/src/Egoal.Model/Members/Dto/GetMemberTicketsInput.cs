using Egoal.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Members.Dto
{
    public class GetMemberTicketsInput : PagedInputDto
    {
        public Guid MemberId { get; set; }
        public Guid? CustomerId { get; set; }
    }
}
