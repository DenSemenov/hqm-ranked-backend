﻿using hqm_ranked_database.DbModels;
using Microsoft.EntityFrameworkCore;

namespace hqm_ranked_backend.Models.DbModels
{
    public class RankedDb : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Avatar> Avatars { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<States> States { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GamePlayer> GamePlayers { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<EventWinners> EventWinners { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Elo> Elos { get; set; }
        public DbSet<Bans> Bans { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<NicknameChanges> NicknameChanges { get; set; }
        public DbSet<ReplayData> ReplayData { get; set; }
        public DbSet<ReplayFragment> ReplayFragments { get; set; }
        public DbSet<ReplayChat> ReplayChats { get; set; }
        public DbSet<ReplayGoal> ReplayGoals { get; set; }
        public DbSet<ReplayHighlight> ReplayHighlights { get; set; }
        public DbSet<Reports> Reports { get; set; }
        public DbSet<PlayerNotification> PlayerNotifications { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<AdminStory> AdminStories { get; set; }
        public DbSet<PatrolDecision> PatrolDecisions { get; set; }
        public DbSet<Music> Music { get; set; }
        public DbSet<PlayerCost> Costs { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<PlayerInvite> PlayerInvites { get; set; }
        public DbSet<TeamPlayer> TeamPlayers { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<GameInvites> GameInvites { get; set; }
        public DbSet<GameInviteVote> GameInviteVotes { get; set; }
        public DbSet<TransferMarket> TransferMarkets { get; set; }
        public DbSet<TransferMarketResponse> TransferMarketResponses { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<PlayerLogin> PlayerLogins { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractSelect> ContractSelects { get; set; }
        public DbSet<ShopItem> ShopItems { get; set; }
        public DbSet<ShopPurchases> ShopPurchases { get; set; }
        public DbSet<WeeklyTourney> WeeklyTourneys { get; set; }
        public DbSet<WeeklyTourneyTeam> WeeklyTourneyTeams { get; set; }
        public DbSet<WeeklyTourneyPlayer> WeeklyTourneyPlayers { get; set; }
        public DbSet<WeeklyTourneyRequest> WeeklyTourneyRequests { get; set; }
        public DbSet<WeeklyTourneyGame> WeeklyTourneyGame { get; set; }
        public DbSet<WeeklyTourneyParty> WeeklyTourneyParties { get; set; }
        public DbSet<WeeklyTourneyPartyPlayer> WeeklyTourneyPartyPlayers { get; set; }


        public RankedDb(DbContextOptions<RankedDb> options)
       : base(options)
        { }
    }
}
