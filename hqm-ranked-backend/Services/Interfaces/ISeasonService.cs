﻿using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface ISeasonService
    {
        Task<List<SeasonViewModel>> GetSeasons(CurrentDivisionRequest request);
        Task<List<Division>> GetDivisions();
        Task<List<SeasonStatsViewModel>> GetSeasonStats(CurrentSeasonStatsRequest request);
        Task<List<SeasonGameViewModel>> GetSeasonLastGames(CurrentSeasonStatsRequest request);
        Task<PlayerViewModel> GetPlayerData(PlayerRequest request);
    }
}
