using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface IEventService
    {
        Task<CurrentEventViewModel> GetCurrentEvent();
        Task CalculateEvents();
    }
}
