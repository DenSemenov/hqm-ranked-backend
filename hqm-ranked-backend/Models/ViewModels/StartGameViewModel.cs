namespace hqm_ranked_backend.Models.ViewModels
{
    public class StartGameViewModel
    {
        public Guid GameId { get; set; }
        public List<StartGamePlayerViewModel> Players { get; set; } = new List<StartGamePlayerViewModel>();
        public int CaptainRed { get; set; }
        public int CaptainBlue { get; set; }
    }

    public class StartGamePlayerViewModel
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public int Count { get; set; }
        public int Reports { get; set; }
    }
}
