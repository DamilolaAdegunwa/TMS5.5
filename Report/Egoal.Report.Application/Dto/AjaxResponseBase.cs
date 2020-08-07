namespace Egoal.Report.Dto
{
    public abstract class AjaxResponseBase
    {
        public string TargetUrl { get; set; }
        public bool Success { get; set; }
        public dynamic Error { get; set; }
        public bool UnAuthorizedRequest { get; set; }
    }
}
