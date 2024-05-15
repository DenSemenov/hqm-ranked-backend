﻿using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface ISeasonService
    {
        Task<List<SeasonViewModel>> GetSeasons();
        Task<List<SeasonStatsViewModel>> GetSeasonStats(CurrentSeasonStatsRequest request);
        Task<List<SeasonGameViewModel>> GetSeasonGames(CurrentSeasonStatsRequest request);
        Task<PlayerViewModel> GetPlayerData(PlayerRequest request);
        Task<GameDataViewModel> GetGameData(GameRequest request);
        Task<Season> GetCurrentSeason();
        Task<int> GetPlayerElo(int id);
        Task<string> GetRules();
        Task<string> GetStorage();

    }
}
