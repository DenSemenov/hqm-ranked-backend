using Newtonsoft.Json;

namespace ReplayHandler.Classes
{
    public class ReplayClass
    {
        public List<ReplayTick> Ticks { get; set; }

        public ReplayClass()
        {
            Ticks = new List<ReplayTick>();
        }
    }

    public class ReplayTick
    {
        [JsonProperty("pn")]
        public uint PacketNumber { get; set; }
        //public bool GameOver { get; set; }
        [JsonProperty("rs")]
        public int RedScore { get; set; }
        [JsonProperty("bs")]
        public int BlueScore { get; set; }
        [JsonProperty("t")]
        public int Time { get; set; }
        //public int GoalMessageTimer { get; set; }
        [JsonProperty("p")]
        public int Period { get; set; }
        [JsonProperty("pc")]
        public List<ReplayPuck> Pucks { get; set; }
        [JsonProperty("pl")]
        public List<ReplayPlayer> Players { get; set; }
        [JsonProperty("m")]
        public List<ReplayMessage> Messages { get; set; }
        [JsonProperty("pil")]
        public List<PlayerInList> PlayersInList { get; set; }

        public ReplayTick()
        {
            Pucks = new List<ReplayPuck>();
            Players = new List<ReplayPlayer>();
            //Messages = new List<ReplayMessage>();
        }
    }

    public class PlayerInList
    {
        [JsonProperty("li")]
        public int? ListIndex { get; set; }
        [JsonProperty("i")]
        public int Index { get; set; }
        [JsonProperty("n")]
        public string Name { get; set; }
        [JsonProperty("t")]
        public ReplayTeam Team { get; set; }
    }

    public class ReplayPuck
    {
        [JsonProperty("i")]
        public int Index { get; set; }
        [JsonProperty("x")]
        public double PosX { get; set; }
        [JsonProperty("y")]
        public double PosY { get; set; }
        [JsonProperty("z")]
        public double PosZ { get; set; }
        [JsonProperty("rx")]
        public double RotX { get; set; }
        [JsonProperty("ry")]
        public double RotY { get; set; }
        [JsonProperty("rz")]
        public double RotZ { get; set; }

        public ReplayPuck()
        {
            PosX = 0;
            PosY = 0;
            PosZ = 0;
            RotX = 0;
            RotY = 0;
            RotZ = 0;
        }
    }

    public class ReplayPlayer
    {
        [JsonProperty("i")]
        public int Index { get; set; }
        [JsonProperty("x")]
        public double PosX { get; set; }
        [JsonProperty("y")]
        public double PosY { get; set; }
        [JsonProperty("z")]
        public double PosZ { get; set; }
        [JsonProperty("rx")]
        public double RotX { get; set; }
        [JsonProperty("ry")]
        public double RotY { get; set; }
        [JsonProperty("rz")]
        public double RotZ { get; set; }
        [JsonProperty("spx")]
        public double StickPosX { get; set; }
        [JsonProperty("spy")]
        public double StickPosY { get; set; }
        [JsonProperty("spz")]
        public double StickPosZ { get; set; }
        [JsonProperty("srx")]
        public double StickRotX { get; set; }
        [JsonProperty("sry")]
        public double StickRotY { get; set; }
        [JsonProperty("srz")]
        public double StickRotZ { get; set; }
        [JsonProperty("ht")]
        public double HeadTurn { get; set; }
        [JsonProperty("bl")]
        public double BodyLean { get; set; }

        public ReplayPlayer()
        {
            PosX = 0;
            PosY = 0;
            PosZ = 0;
            RotX = 0;
            RotY = 0;
            StickPosX = 0;
            StickPosY = 0;
            StickPosZ = 0;
            StickRotX = 0;
            StickRotY = 0;
            StickRotZ = 0;
            HeadTurn = 0;
            BodyLean = 0;
        }
    }

    public class ReplayMessage
    {
        [JsonProperty("rmt")]
        public ReplayMessageType ReplayMessageType { get; set; }
        [JsonProperty("oi")]
        public int? ObjectIndex { get; set; }

        //Chat
        [JsonProperty("pi")]
        public int? PlayerIndex { get; set; }
        [JsonProperty("m")]
        public string? Message { get; set; }

        //Goal
        [JsonProperty("gi")]
        public int? GoalIndex { get; set; }
        [JsonProperty("ai")]
        public int? AssistIndex { get; set; }

        //PlayerUpdate
        [JsonProperty("upi")]
        public int? UpdatePlayerIndex { get; set; }
        [JsonProperty("pn")]
        public string PlayerName { get; set; }
        [JsonProperty("is")]
        public bool InServer { get; set; }
        [JsonProperty("t")]
        public ReplayTeam Team { get; set; }

    }

    public enum ReplayMessageType
    {
        Chat,
        Goal,
        PlayerUpdate
    }

    public enum ReplayTeam
    {
        Red,
        Blue,
        Spectator
    }
}
