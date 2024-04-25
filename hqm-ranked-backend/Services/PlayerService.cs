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
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using static MassTransit.ValidationResultExtensions;

namespace hqm_ranked_backend.Services
{
    public class PlayerService : IPlayerService
    {
        private RankedDb _dbContext;
        public PlayerService(RankedDb dbContext)
        {
            _dbContext = dbContext;
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
            var user = await _dbContext.Players.FirstOrDefaultAsync(x => x.Name == request.Login.Trim());

            if (user != null)
            {
                return new LoginResult
                {
                    Id = Guid.Empty,
                    Success = false,
                    Token = String.Empty,
                };
            }

            var password = Encryption.GetMD5Hash(request.Password.Trim());
            var userRole = await _dbContext.Roles.SingleOrDefaultAsync(x => x.Name == "user");
            var id = Guid.NewGuid();

            await _dbContext.Players.AddAsync(new Player
            {
                Id = id,
                Name = request.Login.Trim(),
                Email = request.Email.Trim(),
                Password = password,
                Role = userRole,
                IsActive = false,
            });
            await _dbContext.SaveChangesAsync();

            var token = Encryption.GetToken(id, userRole.Name == "admin");
            return new LoginResult
            {
                Id = id,
                Success = true,
                Token = token,
                IsExists = false
            };
        }

        public async Task<CurrentUserVIewModel> GetCurrentUser(Guid userId)
        {
            var result = new CurrentUserVIewModel();

            var user = await _dbContext.Players.Include(x=>x.Role).SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                result.Id = user.Id;
                result.Name = user.Name;
                result.Email = user.Email;
                result.Role = user.Role.Name;
            }

            return result;
        }

        public async Task ChangePassword(PasswordChangeRequest request, Guid userId)
        {
            var user = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var encryptedPassword = Encryption.GetMD5Hash(request.Password);
                user.Password = encryptedPassword;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
