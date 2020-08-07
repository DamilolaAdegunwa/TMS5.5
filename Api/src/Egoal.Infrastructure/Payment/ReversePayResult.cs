namespace Egoal.Payment
{
    public class ReversePayResult
    {
        public bool Success { get; set; }
        public bool ShouldRetry { get; set; }
        public string ErrorMessage { get; set; }
    }
}
