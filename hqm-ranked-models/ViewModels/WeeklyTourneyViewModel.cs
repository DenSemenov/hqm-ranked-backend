using hqm_ranked_database.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class WeeklyTourneyViewModel
    {
        public WeeklyTourneyState State {  get; set; }
        public DateTime StartDate { get; set; }
        public WeeklyTourneyRegistrationViewModel? Registration { get; set; }
        public WeeklyTourneyTourneyViewModel? Tourney { get; set; }
    }

    public class WeeklyTourneyTourneyViewModel
    {
        public Guid TourneyId { get; set; }
        public string TourneyName { get; set; }
        public int WeekNumber { get; set; }
        public int Rounds { get; set; }
        public List<WeeklyTourneyGameViewModel> Games { get; set; }
        public List<WeeklyTourneyTeamViewModel> Teams {get; set; }
    }

    public class WeeklyTourneyTeamViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<WeeklyTourneyTeamPlayerViewModel> Players { get; set; }
    }

    public class WeeklyTourneyTeamPlayerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class WeeklyTourneyGameViewModel
    {
        public Guid Id { get; set; }
        public Guid? NextGameId { get; set; }
        public Guid? RedTeamId { get; set; }
        public Guid? BlueTeamId { get; set; }
        public string RedTeamName { get; set; }
        public string BlueTeamName { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
        public int PlayoffType { get; set; }
        public int Index { get; set; }
    }

    public class WeeklyTourneyRegistrationViewModel
    {
        public Guid TourneyId { get; set; }
        public string TourneyName { get; set; }
        public int WeekNumber { get; set; }
        public List<WeeklyTourneyRegistrationPlayerViewModel> Players { get; set; }
    }

    public class WeeklyTourneyRegistrationPlayerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

}
