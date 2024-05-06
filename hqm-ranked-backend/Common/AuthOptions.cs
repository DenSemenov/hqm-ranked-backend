using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace hqm_ranked_backend.Common
{
    public class AuthOptions
    {
        public const string ISSUER = "HqmRankedAuthServer";
        public const string AUDIENCE = "HqmRankedAuthClient";
        const string KEY = "0925bf65-7698-4f48-bcba-f7228d6f88bb"; 
        public const int LIFETIME = 30;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
