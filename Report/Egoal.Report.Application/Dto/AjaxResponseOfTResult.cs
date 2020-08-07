using System;

namespace Egoal.Report.Dto
{
    [Serializable]
    public class AjaxResponse<TResult> : AjaxResponseBase
    {
        public TResult Result { get; set; }
        public AjaxResponse(TResult result)
        {
            Result = result;
            Success = true;
        }

        public AjaxResponse()
        {
            Success = true;
        }

        public AjaxResponse(bool success)
        {
            Success = success;
        }

        public AjaxResponse(object error, bool unAuthorizedRequest = false)
        {
            Error = error;
            UnAuthorizedRequest = unAuthorizedRequest;
            Success = false;
        }
    }
}
