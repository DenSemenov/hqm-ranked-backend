using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace hqm_ranked_backend.Services
{
    public class EventService: IEventService
    {
        private RankedDb _dbContext;
        public EventService(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CurrentEventViewModel> GetCurrentEvent()
        {
            var result = new CurrentEventViewModel();

            var date = DateTime.UtcNow;

            var ev = await _dbContext.Events.Include(x=>x.EventType).FirstOrDefaultAsync(x => x.CreatedOn.Date == date.Date);
            if (ev != null)
            {
                result.Text = String.Format(ev.EventType.Text, ev.X, ev.Y);
                result.Value = ev.X;
                result.Id = ev.Id;

                result.Players = new List<CurrentEventPlayersViewModel>();
            }

            return result;
        }
    }
}
