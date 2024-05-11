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
        public DbSet<Reports> Reports { get; set; }


        public RankedDb(DbContextOptions<RankedDb> options)
       : base(options)
        { }
    }
}
