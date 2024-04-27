using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System.Drawing.Imaging;

namespace hqm_ranked_backend.Common
{
    internal class ApplicationDbInitializer
    {
        private readonly RankedDb _dbContext;
        private IWebHostEnvironment _hostingEnvironment;
        private IImageGeneratorService _imageGeneratorService;
        public ApplicationDbInitializer(RankedDb dbContext, IWebHostEnvironment hostingEnvironment, IImageGeneratorService imageGeneratorService)
        {
            _dbContext = dbContext;
            _hostingEnvironment = hostingEnvironment;
            _imageGeneratorService = imageGeneratorService;
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

            if (!Directory.Exists(_hostingEnvironment.WebRootPath + "/avatars"))
            {
                Directory.CreateDirectory(_hostingEnvironment.WebRootPath + "/avatars");
            }

            var userIds = await _dbContext.Players.Select(x => x.Id).ToListAsync();
            foreach (var userId in userIds)
            {
                var path = _hostingEnvironment.WebRootPath + "/avatars/" + userId + ".png";
                if (!File.Exists(path))
                {
                    var file = _imageGeneratorService.GenerateImage();
                    file.SaveAsPng(path);
                }
            }

        }
    }
}
