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
                    IsApproved = true
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

          


            if (!await _dbContext.EventTypes.AnyAsync())
            {
                _dbContext.EventTypes.Add(new EventType
                {
                    Text = "Score {0} goals",
                    MinX = 17,
                    MaxX = 30,
                    MinY = 0,
                    MaxY = 0,
                });

                _dbContext.EventTypes.Add(new EventType
                {
                    Text = "Do {0} assists",
                    MinX = 17,
                    MaxX = 30,
                    MinY = 0,
                    MaxY = 0,
                });

                _dbContext.EventTypes.Add(new EventType
                {
                    Text = "Win {0} games",
                    MinX = 10,
                    MaxX = 20,
                    MinY = 0,
                    MaxY = 0,
                });

                _dbContext.EventTypes.Add(new EventType
                {
                    Text = "Win {0} games in a row",
                    MinX = 5,
                    MaxX = 10,
                    MinY = 0,
                    MaxY = 0,
                });

                _dbContext.EventTypes.Add(new EventType
                {
                    Text = "Do same count of goals and assists more than 0 {0} games",
                    MinX = 3,
                    MaxX = 10,
                    MinY = 0,
                    MaxY = 0,
                });

                await _dbContext.SaveChangesAsync();
            }

            if (!await _dbContext.Settings.AnyAsync())
            {
                _dbContext.Settings.Add(new Setting
                {
                    NewPlayerApproveRequired = false,
                    NicknameChangeDaysLimit = 30,
                    ReplayStoreDays = 10,
                    Rules = String.Empty
                });
                await _dbContext.SaveChangesAsync();
            }

            if (!await _dbContext.States.AnyAsync(x=>x.Name == "Canceled"))
            {
                _dbContext.States.Add(new States
                {
                    Name = "Canceled"
                });

                await _dbContext.SaveChangesAsync();
            }

            if (!await _dbContext.States.AnyAsync(x => x.Name == "Resigned"))
            {
                _dbContext.States.Add(new States
                {
                    Name = "Resigned"
                });

                await _dbContext.SaveChangesAsync();
            }

            if (!await _dbContext.Rules.AnyAsync())
            {
                _dbContext.Rules.Add(new Rule
                {
                    Title = "Blocking the view",
                    Description = "Blocking the view with a stick by poking the stick into the player's camera"
                });

                _dbContext.Rules.Add(new Rule
                {
                    Title = "Ignoring the 1/3 system",
                    Description = "The player selected by the 1/3 system must be a goalkeeper with his period"
                });

                _dbContext.Rules.Add(new Rule
                {
                    Title = "Gint",
                    Description = "It is forbidden to push the goalkeeper out of the goalie's area"
                });

                _dbContext.Rules.Add(new Rule
                {
                    Title = "Sabotage",
                    Description = "Sabotage is considered to be intentionally scoring a goal in their own goal, intentionally blocking the view with their teammate's stick, intentionally interfering with the movement of the teammate, maliciously using the goaltending position for greater possession of the puck without intending to play \"for the team\""
                });

                _dbContext.Rules.Add(new Rule
                {
                    Title = "AFK",
                    Description = "No attempts to play for more than 30 seconds"
                });

                _dbContext.Rules.Add(new Rule
                {
                    Title = "Offense",
                    Description = "It is forbidden to use statements aimed at insulting or humiliating players"
                });

                _dbContext.Rules.Add(new Rule
                {
                    Title = "Anti fair play",
                    Description = "It is necessary to compensate for a goal that was scored in violation of the rules"
                });

                _dbContext.Rules.Add(new Rule
                {
                    Title = "Multiple accounts",
                    Description = "It is forbidden to have more than one account, in case of loss of the password, you must write to the administrator"
                });

                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.SaveChangesAsync();

        }
    }
}
