using System;

namespace Egoal.Report.Dto
{
    [Serializable]
    public class AjaxResponse : AjaxResponse<object>
    {
        public AjaxResponse()
        {

        }

        public AjaxResponse(bool success)
            : base(success)
        {

        }

        public AjaxResponse(object result)
            : base(result)
        {

        }

        public AjaxResponse(object error, bool unAuthorizedRequest = false)
            : base(error, unAuthorizedRequest)
        {

        }
    }
}
