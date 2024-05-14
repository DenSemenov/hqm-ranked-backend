using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public uint PacketNumber { get; set; }
        //public bool GameOver { get; set; }
        public int RedScore { get; set; }
        public int BlueScore { get; set; }
        public int Time { get; set; }
        //public int GoalMessageTimer { get; set; }
        public int Period { get; set; }
        public List<ReplayPuck> Pucks { get; set; }
        public List<ReplayPlayer> Players { get; set; }
        public List<ReplayMessage> Messages { get; set; }
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
        public int? ListIndex { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public ReplayTeam Team { get; set; }
    }

    public class ReplayPuck
    {
        public int Index { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double PosZ { get; set; }
        public double RotX { get; set; }
        public double RotY { get; set; }
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
        public int Index { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double PosZ { get; set; }
        public double RotX { get; set; }
        public double RotY { get; set; }
        public double RotZ { get; set; }
        public double StickPosX { get; set; }
        public double StickPosY { get; set; }
        public double StickPosZ { get; set; }
        public double StickRotX { get; set; }
        public double StickRotY { get; set; }
        public double StickRotZ { get; set; }
        public double HeadTurn { get; set; }
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
        public ReplayMessageType ReplayMessageType { get; set; }
        public int? ObjectIndex { get; set; }

        //Chat
        public int? PlayerIndex { get; set; }
        public string? Message { get; set; }

        //Goal
        public int? GoalIndex { get; set; }
        public int? AssistIndex { get; set; }

        //PlayerUpdate
        public int? UpdatePlayerIndex { get; set; }
        public string PlayerName { get; set; }
        public bool InServer { get; set; }
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
