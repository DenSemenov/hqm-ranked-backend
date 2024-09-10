using hqm_ranked_models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_services.Interfaces
{
    public interface IWeeklyTourneyService
    {
        Task CreateTourney();
        Task RandomizeTourney();
        Task<WeeklyTourneyViewModel> GetCurrentWeeklyTournament();
        Task WeeklyTourneyRegister(int userId);
    }
}
