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

        public RankedDb(DbContextOptions<RankedDb> options)
       : base(options)
        { }
    }
}
