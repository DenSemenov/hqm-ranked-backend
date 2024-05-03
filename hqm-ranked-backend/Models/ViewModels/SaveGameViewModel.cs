namespace hqm_ranked_backend.Models.ViewModels
{
    public class SaveGameViewModel
    {
        public string Mvp { get; set; }
        public List<SaveGamePlayerViewModel> Players { get; set; } = new List<SaveGamePlayerViewModel>();

    }

    public class SaveGamePlayerViewModel
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int Total { get; set; }
    }
}
