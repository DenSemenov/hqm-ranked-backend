using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace hqm_ranked_backend.Services
{
    public class AdminService: IAdminService
    {
        private RankedDb _dbContext;
        public AdminService(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AdminServerViewModel>> GetServers()
        {
            var result = await _dbContext.Servers.OrderBy(x => x.CreatedOn).Select(x => new AdminServerViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Token = x.Token,
            }).ToListAsync();

            return result;
        }

        public async Task AddServer(AddServerRequest request)
        {
            await _dbContext.Servers.AddAsync(new Server
            {
                Name = request.Name,
                Token = Guid.NewGuid().ToString(),
            });
            await _dbContext.SaveChangesAsync();
        }
        public async Task RemoveServer(RemoveServerRequest request)
        {
            var server = await _dbContext.Servers.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (server != null)
            {
                _dbContext.Servers.Remove(server);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
