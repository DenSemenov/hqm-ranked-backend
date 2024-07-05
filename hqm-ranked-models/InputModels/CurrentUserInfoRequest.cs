using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hqm_ranked_models.InputModels
{
    public class CurrentUserInfoRequest
    {
        public string Ip { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }

    }
}
