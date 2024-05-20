using ReplayHandler.Classes;

namespace hqm_ranked_backend.Models.DTO
{
    public class ReplayCalculatedData
    {
        public List<ReplayCalculatedChat> Chats { get; set; } = new List<ReplayCalculatedChat>();
        public List<ReplayCalculatedGoal> Goals { get; set; } = new List<ReplayCalculatedGoal>();
        public List<ReplayCalculatedPossession> Possession { get; set; } = new List<ReplayCalculatedPossession>();
        public List<ReplayCalculatedShot> Shots { get; set; } = new List<ReplayCalculatedShot>();
        public List<ReplayCalculatedSave> Saves { get; set; } = new List<ReplayCalculatedSave>();
        public List<ReplayCalculatedGoaliePosition> Goalies { get; set; } = new List<ReplayCalculatedGoaliePosition>();
        public List<ReplayCalculatedPause> Pauses { get; set; } = new List<ReplayCalculatedPause>();
    }

    public class ReplayCalculatedChat
    {
        public uint Packet { get; set; }
        public string Text { get; set; }
    }

    public class ReplayCalculatedGoal
    {
        public uint Packet { get; set; }
        public string GoalBy { get; set; }
        public int Period { get; set; }
        public int Time { get; set; }
    }

    public class ReplayCalculatedPossession
    {
        public string Name { get; set; }
        public int Touches { get; set; }
    }

    public class ReplayCalculatedShot
    {
        public uint Packet { get; set; }
        public string Name { get; set; }
    }
    public class ReplayCalculatedSave
    {
        public uint Packet { get; set; }
        public string Name { get; set; }
    }
    public class ReplayCalculatedGoaliePosition
    {
        public uint StartPacket { get; set; }
        public uint EndPacket { get; set; }
        public ReplayTeam Team { get; set; }
        public string Name { get; set; }
    }

    public class ReplayCalculatedPause
    {
        public uint StartPacket { get; set; }
        public uint EndPacket { get; set; }

    }
}
