﻿namespace hqm_ranked_backend.Models.InputModels
{
    public class ServerUpdateRequest
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public int PlayerCount { get; set; }
    }
}
