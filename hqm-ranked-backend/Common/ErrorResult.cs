namespace hqm_ranked_backend.Common
{
    public class ErrorResult
    {
        public List<string> Messages { get; set; } = new();

        public string? Source { get; set; }
        public string? Exception { get; set; }
        public string? ErrorId { get; set; }
        public string? SupportMessage { get; set; }
        public int StatusCode { get; set; }
        public string Error { get; set; }
        public string StackTrace { get; set; }
    }
}
