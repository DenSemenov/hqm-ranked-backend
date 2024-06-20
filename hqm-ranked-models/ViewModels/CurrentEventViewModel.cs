namespace hqm_ranked_backend.Models.ViewModels
{
    public class CurrentEventViewModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public int Value { get; set; }
        public List<CurrentEventPlayersViewModel> Players { get; set; }
        public string Left { get; set; }
    }

    public class CurrentEventPlayersViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CurrentValue { get; set; }
    }
}
