namespace hqm_ranked_backend.Models.InputModels
{
    public class BanUnbanRequest
    {
        public bool IsBanned { get; set; }
        public int Id { get; set; }
        public int Days { get; set; }
    }
}
