using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.InputModels
{
    public class AddServerRequest
    {
        public string Name { get; set; }
        public InstanceType InstanceType { get; set; }
    }
}
