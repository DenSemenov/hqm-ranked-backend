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
            var password = Encryption.GetMD5Hash(request.Password);

            var player = await _dbContext.Players.SingleOrDefaultAsync(x => x.Name == request.Login && x.Password == password);
            if (player != null)
            {
                var now = DateTime.UtcNow;
                var jwt = new JwtSecurityToken(
                        issuer: AuthOptions.ISSUER,
                        audience: AuthOptions.AUDIENCE,
                        notBefore: now,
                        expires: now.Add(TimeSpan.FromDays(AuthOptions.LIFETIME)),
                        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
                jwt.Payload["id"] = player.Id;

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                return new LoginResult
                {
                    Success = true,
                    Token = encodedJwt
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<CurrentUserVIewModel> GetCurrentUser(Guid userId)
        {
            var result = new CurrentUserVIewModel();

            var user = await _dbContext.Players.SingleOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                result.Id = user.Id;
                result.Name = user.Name;
                result.Email = user.Email;
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
