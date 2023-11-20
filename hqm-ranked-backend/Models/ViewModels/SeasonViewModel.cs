namespace hqm_ranked_backend.Models.ViewModels
{
    public class SeasonViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public Guid DivisionId { get; set; }
    }
}
