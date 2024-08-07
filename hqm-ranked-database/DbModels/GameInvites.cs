﻿using hqm_ranked_backend.Common;

namespace hqm_ranked_backend.Models.DbModels
{
    public class GameInvites : AuditableEntity<Guid>
    {
        public Team InvitedTeam { get; set; }
        public DateTime Date {  get; set; }
        public List<Game> Games { get; set; } = new List<Game>();
        public int GamesCount { get; set; }
        public ICollection<GameInviteVote> GameInviteVotes { get; set; }
        public bool NotificationSent { get; set; } = false;
    }
}
