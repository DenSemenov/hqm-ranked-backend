using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.ViewModels
{
    public class AdminServerViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public InstanceType InstanceType { get; set; }
    }
}
