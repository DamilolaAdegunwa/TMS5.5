using Egoal.Annotations;
using Egoal.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Egoal.Tickets.Dto
{
    public class GetTicketFullInfoInput : IValidatableObject
    {
        public long? Id { get; set; }

        [MaximumLength(50)]
        public string TicketCode { get; set; }

        public string CertNo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Id.HasValue && TicketCode.IsNullOrEmpty() && CertNo.IsNullOrEmpty())
            {
                yield return new ValidationResult("查询条件不能为空");
            }
        }
    }
}
