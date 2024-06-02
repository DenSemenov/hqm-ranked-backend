namespace hqm_ranked_backend.Models.ViewModels
{
    public class AdminStoryViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public bool Expiration { get; set; }
        public string Link { get; set; }
    }
}
