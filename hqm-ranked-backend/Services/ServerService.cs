using hqm_ranked_backend.Models.DbModels;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hqm_ranked_backend.Services
{
    public class ServerService : IServerService
    {
        private RankedDb _dbContext;
        public ServerService(RankedDb dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ActiveServerViewModel>> GetActiveServers()
        {
            var date = DateTime.UtcNow.AddMinutes(-15);
            var servers = await _dbContext.Servers
                .Where(x => x.CreatedOn > date || (x.LastModifiedOn != null && x.LastModifiedOn > date))
                .Select(x => new ActiveServerViewModel
                {
                    Name = x.Name,
                    Count = x.PlayerCount
                })
                .ToListAsync();

            return servers;
        }

        public async Task ServerUpdate(ServerUpdateRequest request)
        {
            var server = await _dbContext.Servers.SingleOrDefaultAsync(x=>x.Name == request.Name);
            if (server != null)
            {
                server.PlayerCount = request.PlayerCount;
            }
            else
            {
                await _dbContext.Servers.AddAsync(new Server
                {
                    Name = request.Name,
                    PlayerCount = request.PlayerCount
                });
            }
            
            await _dbContext.SaveChangesAsync();
        }
    }
}
