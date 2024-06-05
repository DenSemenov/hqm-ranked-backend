namespace hqm_ranked_backend.Models.InputModels
{
    public class PlayerReportRequest
    {
        public int Id { get; set; }
        public Guid ReasonId { get; set; }
        public int Tick { get; set; }
    }
}
