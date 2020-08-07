using Egoal.Annotations;
using Egoal.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Egoal.Members.Dto
{
    public class RegistMemberInput
    {
        [Display(Name = "会员姓名")]
        [MustFillIn]
        public string Name { get; set; }

        public string Sex { get; set; }

        [Display(Name = "手机号码")]
        [MustFillIn]
        public string Mobile { get; set; }

        public string Birth { get; set; }

        [Display(Name = "验证码")]
        [MustFillIn]
        public string VerificationCode { get; set; }
    }
}
