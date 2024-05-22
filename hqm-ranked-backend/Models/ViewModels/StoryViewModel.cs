﻿namespace hqm_ranked_backend.Models.ViewModels
{
    public class StoryViewModel
    {
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public List<StoryGoalViewModel> Goals { get; set; }
    }

    public class StoryGoalViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Guid ReplayId { get; set; }  
        public uint Packet {  get; set; }   

    }
}
