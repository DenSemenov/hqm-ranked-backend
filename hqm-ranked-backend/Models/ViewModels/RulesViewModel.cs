namespace hqm_ranked_backend.Models.ViewModels
{
    public class RulesViewModel
    {
        public string Text { get; set; }
        public List<RulesItemViewModel> Rules { get; set; } 
    }

    public class RulesItemViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
