using System.Security.Claims;

namespace hqm_ranked_backend.Common
{
    public static class UserHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var id = user.Claims.FirstOrDefault(x => x.Type == "id").Value;

            return Int32.Parse(id);
        }
    }
}
