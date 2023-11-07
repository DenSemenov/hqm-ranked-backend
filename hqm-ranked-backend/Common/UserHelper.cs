using System.Security.Claims;

namespace hqm_ranked_backend.Common
{
    public static class UserHelper
    {
        public static Guid GetUserId(ClaimsPrincipal user)
        {
            var id = user.Claims.FirstOrDefault(x => x.Type == "id").Value;

            return Guid.Parse(id);
        }
    }
}
