using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using hqm_ranked_backend.Common;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace hqm_ranked_backend.Helpers
{
    public static class Encryption
    {
        public static string GetMD5Hash(string password)
        {
            using (var hashAlg = MD5.Create())
            {
                byte[] hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("X2"));
                }
                return builder.ToString();
            }
        }

        public static string GetToken(int userId, bool isAdmin)
        {
            var now = DateTime.UtcNow;
            var claims = new List<Claim>();

            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromDays(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256),
                    claims: claims
                    );
            jwt.Payload["id"] = userId;

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt.ToString();
        }
    }
}
