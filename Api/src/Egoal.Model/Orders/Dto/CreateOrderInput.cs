using Egoal.Annotations;
using Egoal.Members;
using Egoal.Scenics.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Egoal.Orders.Dto
{
    public class CreateOrderInput
    {
        public const string SignKey = "B885EA36-A238-44BB-A375-040500431DDB";

        public DateTime TravelDate { get; set; }
        public string ContactName { get; set; }

        [Display(Name = "手机号码")]
        [MobileNumber]
        public string ContactMobile { get; set; }

        [Display(Name = "证件号码")]
        public string ContactCert { get; set; }

        public int? ContactCertTypeId { get; set; }

        public int? PromoterId { get; set; }
        public int? CashierId { get; set; }
        public int? CashPcid { get; set; }
        public int? SalePointId { get; set; }
        public int? ParkId { get; set; }
        public string Sign { get; set; }

        public List<OrderItemDto> Items { get; set; }
    }

    public class OrderItemDto
    {
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }
        public List<OrderTouristDto> Tourists { get; set; }
        public List<GroundChangCiDto> GroundChangCis { get; set; }
    }

    public class OrderTouristDto
    {
        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "手机号码")]
        [MobileNumber]
        public string Mobile { get; set; }

        [Display(Name = "证件号码")]
        public string CertNo { get; set; }

        [Display(Name = "证件类型")]
        public int? CertTypeId { get; set; }
    }
}
