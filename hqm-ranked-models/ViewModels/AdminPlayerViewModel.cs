using hqm_ranked_database.DbModels;

namespace hqm_ranked_backend.Models.ViewModels
{
    public class AdminPlayerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsBanned { get; set; }
        public List<AdminPlayerLoginViewModel> Logins { get; set; }

    }

    public class AdminPlayerLoginViewModel
    {
        public DateTime Date { get; set; }
        public string Ip { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public LoginInstance LoginInstance { get; set; }

    }
}
