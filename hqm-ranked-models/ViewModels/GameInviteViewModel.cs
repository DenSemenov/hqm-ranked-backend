namespace hqm_ranked_backend.Models.ViewModels
{
    public class GameInviteViewModel
    {
        public Guid Id { get; set; }
        public bool IsCurrentTeam { get; set; }
        public DateTime Date { get; set; }
        public int GamesCount { get; set; }
        public List<GameInviteVoteViewModel> Votes { get; set; }
        public int VotesCount { get; set; }
        public List<GameInviteAnotherTeamViewModel> OtherTeams { get; set; }
    }

    public class GameInviteVoteViewModel
    {
        public int Id { get; set; } 
    }

    public class GameInviteAnotherTeamViewModel
    {
        public string Name { get; set; }
        public int Votes { get; set; }
    }
}
