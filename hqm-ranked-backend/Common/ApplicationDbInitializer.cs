using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System.Drawing.Imaging;

namespace hqm_ranked_backend.Common
{
    internal class ApplicationDbInitializer
    {
        private readonly RankedDb _dbContext;
        private IImageGeneratorService _imageGeneratorService;
        private IStorageService _storageService;
        public ApplicationDbInitializer(RankedDb dbContext, IImageGeneratorService imageGeneratorService, IStorageService storageService)
        {
            _dbContext = dbContext;
            _imageGeneratorService = imageGeneratorService;
            _storageService = storageService;
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

            var existsFiles = await _storageService.GetAllFileNames();
            var settings = await _dbContext.Settings.FirstOrDefaultAsync();

            var userIds = await _dbContext.Players.Select(x => x.Id).ToListAsync();
            foreach (var userId in userIds)
            {
                var path = String.Format("images/{0}.png", userId);

                if (!existsFiles.Contains(path))
                {
                    var file = _imageGeneratorService.GenerateImage();
                    var strm = new MemoryStream();
                    file.SaveAsPng(strm);

                    await _storageService.UploadFileStream(path, strm); 
                }
            }

            await _dbContext.SaveChangesAsync();

        }
    }
}
