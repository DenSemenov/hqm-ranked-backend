﻿namespace hqm_ranked_backend.Models.InputModels
{
    public class AddGoalRequest
    {
        public string Token { get; set; }
        public Guid GameId { get; set; }
        public int Team { get; set; }
        public int? Scorer { get; set; }
        public int? Assist { get; set; }
    }
}
