namespace hqm_ranked_backend.Models.ViewModels
{
    public class StartGameViewModel
    {
        public Guid GameId { get; set; }
        public List<StartGamePlayerViewModel> Players { get; set; } = new List<StartGamePlayerViewModel>();
        public Guid CaptainRed { get; set; }
        public Guid CaptainBlue { get; set; }
    }

    public class StartGamePlayerViewModel
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
    }
}
