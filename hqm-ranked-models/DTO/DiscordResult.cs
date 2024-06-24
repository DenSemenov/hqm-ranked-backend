using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.DTO
{
    public class DiscordResult
    {
        public string id { get; set; }
        public string username { get; set; }
        public string avatar { get; set; }
        public object avatar_decoration { get; set; }
        public string discriminator { get; set; }
        public int public_flags { get; set; }
        public int flags { get; set; }
        public object banner { get; set; }
        public object banner_color { get; set; }
        public object accent_color { get; set; }
        public string locale { get; set; }
        public bool mfa_enabled { get; set; }
        public int premium_type { get; set; }
    }
}
