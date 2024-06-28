using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.ViewModels
{
    public class PlayerWarningViewModel
    {
        public WarningType Type { get; set; }
        public string Message { get; set; }
    }

    public enum WarningType
    {
        DiscordNotConnected,
    }
}
