using System;

namespace Egoal.Thirdparties.BigData.Dto
{
    public class StatFlowInput : InputBase
    {
        public DateTime start_date { get; set; }

        public override void Validate()
        {
        }
    }
}
