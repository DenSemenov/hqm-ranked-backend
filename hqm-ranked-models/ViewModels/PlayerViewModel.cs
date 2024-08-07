﻿using hqm_ranked_backend.Common;
using hqm_ranked_database.DbModels;

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
        public int Cost { get; set; }
        public PlayerLastSeasonViewModel CurrentSeasonData { get; set; }
        public List<PlayerLastGamesViewModel> LastGames {  get; set; }
        public List<PlayerSeasonsViewModel> LastSeasons { get; set; } = new List<PlayerSeasonsViewModel>();
        public PlayerCalcDataViewModel CalcData { get; set; }
        public List<int> PlayerPoints { get; set; } = new List<int>();
        public List<string> OldNicknames { get; set; } = new List<string>();
        public List<PlayerAwardViewModel> Awards { get; set; }
        public PlayerCalcStatsViewModel CalcStats {  get; set; }
    }

    public class PlayerCalcStatsViewModel
    {
        public double Mvp { get; set; }
        public double Winrate { get; set; }
        public double Goals { get; set; }
        public double Assists { get; set; }
        public double Shots { get; set; }
        public double Saves { get; set; }
    }

    public class PlayerAwardViewModel
    {
        public DateTime Date { get; set; }
        public AwardType AwardType { get; set; }
        public int? Count { get; set; }
        public string? SeasonName { get; set; }
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
        public int Score { get; set; }
        public DateTime Date { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public InstanceType InstanceType { get; set; }
        public Guid? RedTeamId { get; set; }
        public Guid? BlueTeamId { get; set; }
        public string RedTeamName { get; set; }
        public string BlueTeamName { get; set; }
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
