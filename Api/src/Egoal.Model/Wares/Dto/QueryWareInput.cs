using Egoal.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Wares.Dto
{
    public class QueryWareInput : PagedInputDto
    {
        public string Name { get; set; }

        public string WareCode { get; set; }

        public string Zjf { get; set; }

        public string BarCode { get; set; }

        public int? WareTypeId { get; set; }

        public int? MerchantId { get; set; }

        public Guid? SupplierId { get; set; }
    }
}
