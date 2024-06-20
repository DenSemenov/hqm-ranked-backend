namespace hqm_ranked_backend.Models.InputModels
{
    public class ReportDecisionRequest
    {
        public Guid Id { get; set; }
        public bool IsReported { get; set; }
    }
}
