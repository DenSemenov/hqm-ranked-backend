﻿using hqm_ranked_models.InputModels;
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
        Task RandomizeTourneyNextStage(int stage);
        Task<Guid?> GetCurrentTourneyId();
        Task<Guid?> GetCurrentRunningTourneyId();
        Task<List<WeeklyTourneyItemViewModel>> GetWeeklyTourneys();
        Task<WeeklyTourneyViewModel> GetWeeklyTournament(WeeklyTourneyIdRequest request);
        Task WeeklyTourneyRegister(int userId);
        Task WeeklyTourneyInvite(int userId, int invitedId);
        Task WeeklyTourneyAcceptDeclineInvite(int userId, WeeklyTourneyAcceptDeclineInvite request);
    }
}
