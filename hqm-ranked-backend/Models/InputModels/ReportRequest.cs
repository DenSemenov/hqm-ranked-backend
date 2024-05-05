namespace hqm_ranked_backend.Models.InputModels
{
    public class ReportRequest
    {
        public string Token { get; set; }
        public int FromId { get; set; }
        public int ToId { get; set; }
    }
}
