using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Models.ViewModels;
using hqm_ranked_database.DbModels;
using hqm_ranked_models.InputModels;
using hqm_ranked_models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace hqm_ranked_backend.Services.Interfaces
{
    public interface ITeamsService
    {
        Task<TeamsStateViewModel> GetTeamsState(int? userId);
        Task CreateTeam(string name, int userId);
        Task<List<FreeAgentViewModel>> GetFreeAgents(int? userId);
        Task LeaveTeam(int userId);
        Task InvitePlayer(int userId, int invitedId);
        Task CancelInvite(int userId, Guid inviteId);
        Task CancelExpiredInvites();
        Task<List<PlayerInviteViewModel>> GetInvites(int userId);
        Task ApplyPlayerInvite(int userId, Guid inviteId);
        Task DeclinePlayerInvite(int userId, Guid inviteId);
        Task<TeamViewModel> GetTeam(Guid teamId);
        Task SellPlayer(int userId, int playerId);
        Task UploadAvatar(int userId, IFormFile file);
        Task MakeCaptain(int userId, int playerId);
        Task MakeAssistant(int userId, int playerId);
        Task<string> CreateGameInvite(int userId, DateTime date, int count);
        Task RemoveGameInvite(int userId, Guid inviteId);
        Task<List<GameInviteViewModel>> GetGameInvites(int userId);
        Task VoteGameInvite(int userId, Guid inviteId);
        Task<TeamsStatsViewModel> GetTeamsStats(CurrentSeasonStatsRequest request);
        Task CreateTransferMarket(int userId, List<Position> positions, int budget);
        Task<List<TransferMarketViewModel>> GetTransferMarket();
        Task RemoveTransferMarket(RemoveTransferMarketRequest request);
        Task AskToJoinTeam(int userId, Guid transferMarketId, List<Position> positions);
    }
}
