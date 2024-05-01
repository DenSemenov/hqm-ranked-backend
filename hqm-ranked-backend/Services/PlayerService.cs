using hqm_ranked_backend.Common;
using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using static MassTransit.ValidationResultExtensions;

namespace hqm_ranked_backend.Services
{
    public class PlayerService : IPlayerService
    {
        private RankedDb _dbContext; 
        private IWebHostEnvironment _hostingEnvironment;
        private IImageGeneratorService _imageGeneratorService;
        public PlayerService(RankedDb dbContext, IWebHostEnvironment hostingEnvironment, IImageGeneratorService imageGeneratorService)
        {
            _dbContext = dbContext;
            _hostingEnvironment = hostingEnvironment;
            _imageGeneratorService = imageGeneratorService;
        }
        public async Task<LoginResult?> Login(LoginRequest request)
        {
            var password = Encryption.GetMD5Hash(request.Password.Trim());

            var player = await _dbContext.Players.Include(x=>x.Role).SingleOrDefaultAsync(x => x.Name == request.Login.Trim() && x.Password == password);
            if (player != null)
            {
                var token = Encryption.GetToken(player.Id, player.Role.Name == "admin");

                return new LoginResult
                {
                    Id = player.Id,
                    Success = true,
                    Token = token
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<LoginResult?> Register(RegistrationRequest request)
        {
            try
            {
                var user = await _dbContext.Players.FirstOrDefaultAsync(x => x.Name == request.Login.Trim());

                if (user != null)
                {
                    return new LoginResult
                    {
                        Id = 0,
                        Success = false,
                        Token = String.Empty,
                    };
                }

                var password = Encryption.GetMD5Hash(request.Password.Trim());
                var userRole = await _dbContext.Roles.SingleOrDefaultAsync(x => x.Name == "user");

                var approveRequired = _dbContext.Settings.FirstOrDefault().NewPlayerApproveRequired;

                var entity = await _dbContext.Players.AddAsync(new Player
                {
                    Id = _dbContext.Players.Max(x=>x.Id)+1,
                    Name = request.Login.Trim(),
                    Email = request.Email.Trim(),
                    Password = password,
                    Role = userRole,
                    IsApproved = approveRequired ? false : true,
                });
                await _dbContext.SaveChangesAsync();

                var token = Encryption.GetToken(entity.Entity.Id, userRole.Name == "admin");

                var path = _hostingEnvironment.WebRootPath + "/avatars/" + entity.Entity.Id + ".png";
                if (!File.Exists(path))
                {
                    var file = _imageGeneratorService.GenerateImage();
                    file.SaveAsPng(path);
                }


                return new LoginResult
                {
                    Id = entity.Entity.Id,
                    Success = true,
                    Token = token,
                    IsExists = false
                };
            }
            catch (Exception ex)
            {
                return new LoginResult
                {
                    Id = 0,
                    Success = false,
                    Token = ex.Message,
                };
            }
        }

        public async Task<CurrentUserVIewModel> GetCurrentUser(int userId)
        {
            var result = new CurrentUserVIewModel();

            var user = await _dbContext.Players.Include(x=>x.Bans).Include(x=>x.Role).SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                result.Id = user.Id;
                result.Name = user.Name;
                result.Email = user.Email;
                result.Role = user.Role.Name;

                var approveRequired = _dbContext.Settings.FirstOrDefault().NewPlayerApproveRequired;

                result.IsApproved = approveRequired ? result.IsApproved : true;
                result.IsBanned = user.Bans.Any(x => x.CreatedOn.AddDays(x.Days) >= DateTime.UtcNow);
            }

            return result;
        }

        public async Task ChangePassword(PasswordChangeRequest request, int userId)
        {
            var user = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var encryptedPassword = Encryption.GetMD5Hash(request.Password);
                user.Password = encryptedPassword;
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<string> ChangeNickname(NicknameChangeRequest request, int userId)
        {
            var result = String.Empty;

            var user = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var settings = await _dbContext.Settings.FirstOrDefaultAsync();
                var countDays = settings.NicknameChangeDaysLimit;

                if (_dbContext.NicknameChanges.Any(x => x.Player == user && x.CreatedOn.AddDays(countDays) > DateTime.UtcNow))
                {
                    result = String.Format("You can change nickname each {0} days", countDays);
                }
                else
                {
                    _dbContext.NicknameChanges.Add(new NicknameChanges
                    {
                        Player = user,
                        OldNickname = user.Name
                    });
                    user.Name = request.Name;

                    result = "Nickname successfully changed";
                    await _dbContext.SaveChangesAsync();
                }
            }

            return result;
        }
    }
}
