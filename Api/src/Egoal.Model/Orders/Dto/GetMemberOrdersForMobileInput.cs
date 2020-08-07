using Egoal.Application.Services.Dto;
using System;

namespace Egoal.Orders.Dto
{
    public class GetMemberOrdersForMobileInput : PagedInputDto
    {
        public Guid MemberId { get; set; }
        public Guid? CustomerId { get; set; }
        public bool? IsUsable { get; set; }
        public bool? IsNotPaid { get; set; }
    }
}
