namespace AgriLogic.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public ErrorViewModel(string requestId)
        {
            RequestId = requestId;
        }
    }
}
