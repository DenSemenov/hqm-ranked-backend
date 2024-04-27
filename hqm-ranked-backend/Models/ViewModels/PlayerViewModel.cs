namespace hqm_ranked_backend.Models.ViewModels
{
    public class PlayerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Games { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Points { get; set; }
        public PlayerLastSeasonViewModel CurrentSeasonData { get; set; }
        public List<PlayerLastGamesViewModel> LastGames {  get; set; }
        public List<PlayerSeasonsViewModel> LastSeasons { get; set; } = new List<PlayerSeasonsViewModel>();
        public PlayerCalcDataViewModel CalcData { get; set; }
    }

    public class PlayerLastSeasonViewModel
    {
        public int Position { get; set; }
        public int Games { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Points { get; set; }
        public int Elo { get; set; }
    }

    public class PlayerSeasonsViewModel
    {
        public string Name { get; set; }    
        public int Place { get; set; }
    }


    public class PlayerLastGamesViewModel
    {
        public Guid GameId { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
        public DateTime Date { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public List<GameDataPlayerViewModel> Players { get; set; }
    }

    public class PlayerCalcDataViewModel
    {
        public int Shots { get; set; }
        public int Dribbling { get; set; }
        public int Passes { get; set; }
        public int LongShots { get; set; }
        public int Tackling { get; set; }
        public int Pressing { get; set; }
        public int Blocks { get; set; }
        public int Interception { get; set; }
        public int HighBlocks { get; set; }
        public int LowBlocks { get; set; }
        public int OneByOne { get; set; }
        public int GateLeaving { get; set; }
    }
}
