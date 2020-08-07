using Egoal.Annotations;
using Egoal.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Egoal.Tickets.Dto
{
    public class CheckTicketInput : IValidatableObject
    {
        [MaximumLength(50)]
        public string TicketCode { get; set; }

        public string CertNo { get; set; }
        public int GroundId { get; set; }
        public int GateGroupId { get; set; }
        public int GateId { get; set; }
        public ConsumeType ConsumeType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TicketCode.IsNullOrEmpty() && CertNo.IsNullOrEmpty())
            {
                yield return new ValidationResult("无效票");
            }
        }
    }
}
