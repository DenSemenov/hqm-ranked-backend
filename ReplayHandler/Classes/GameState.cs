using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayHandler.Classes
{
    public class GameState
    {
        public uint PacketNumber { get; set; }

        public uint RedScore { get; set; }

        public uint BlueScore { get; set; }

        public uint Period { get; set; }

        public bool GameOver { get; set; }

        public uint Time { get; set; }

        public uint GoalMessageTimer { get; set; }

        public List<ObjectPacket> ObjectPackets { get; set; } =new List<ObjectPacket>();
        public List<GameEvent> Events { get; set; } = new List<GameEvent>();
    }

    public class ObjectPacket
    {
        public uint Index;

        public byte Type = byte.MaxValue;
    }

    public abstract class GameEvent
    {
        public byte Type;
    }
}
