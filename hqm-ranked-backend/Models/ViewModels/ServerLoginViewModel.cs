namespace hqm_ranked_backend.Models.ViewModels
{
    public class ServerLoginViewModel
    {
        public Guid Id { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
