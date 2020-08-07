using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Egoal.Orders.Dto
{
    public class SelfHelpTicketGroundListDto
    {
        [Display(Name = "区域")]
        public string GroundName { get; set; }

        [Display(Name = "总次数")]
        public int TotalNum { get; set; }

        [Display(Name = "剩余次数")]
        public int SurplusNum { get; set; }

        [Display(Name = "最后检票时间")]
        public string LastCheckTime { get; set; }

        public DateTime? LastInCheckTime { get; set; }

        public DateTime ETime { get; set; }
    }
}
