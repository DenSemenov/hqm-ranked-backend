using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace hqm_ranked_backend.Common
{
    internal class ApplicationDbInitializer
    {
        private readonly RankedDb _dbContext;

        public ApplicationDbInitializer(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Initialize()
        {
            if (_dbContext.Database.GetMigrations().Any())
            {
                if ((await _dbContext.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }
            }

            if (!await _dbContext.Roles.AnyAsync())
            {
                _dbContext.Roles.Add(new Role
                {
                    Name = "admin",
                });
                _dbContext.Roles.Add(new Role
                {
                    Name = "user",
                });

                await _dbContext.SaveChangesAsync();
            }

            if (!await _dbContext.Players.AnyAsync())
            {
                _dbContext.Players.Add(new Player
                {
                    Name = "Admin",
                    Password = Encryption.GetMD5Hash("Admin"),
                    Email = String.Empty,
                    Role = _dbContext.Roles.FirstOrDefault(x => x.Name == "admin"),
                    IsActive = true,
                });

                await _dbContext.SaveChangesAsync();
            }

            if (!await _dbContext.Seasons.AnyAsync())
            {
                _dbContext.Seasons.Add(new Season
                {
                    Name = "Season 1",
                    DateStart = DateTime.UtcNow.Date,
                    DateEnd = DateTime.UtcNow.AddMonths(3).Date
                });


                await _dbContext.SaveChangesAsync();
            }

            if (!await _dbContext.States.AnyAsync())
            {
                _dbContext.States.Add(new States
                {
                    Name = "Pick"
                });

                _dbContext.States.Add(new States
                {
                    Name = "Live"
                });

                _dbContext.States.Add(new States
                {
                    Name = "Ended"
                });

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
