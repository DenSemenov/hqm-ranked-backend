using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayHandler.Classes
{
    public class HQMServerPlayer
    {
        [JsonProperty("n")]
        public string name { get; set; }
        [JsonProperty("ts")]
        public Tuple<int?, HQMTeam?>? team_and_skater { get; set; }
        [JsonProperty("i")]
        public int? index { get; set; }
    }


    public enum HQMTeam
    {
        Red,
        Blue,
    }

    public class HQMObject
    {
        [JsonProperty("t")]
        public int type { get; set; }
        [JsonProperty("i")]
        public int index { get; set; }
        [JsonProperty("px")]
        public float pos_x { get; set; }
        [JsonProperty("py")]
        public float pos_y { get; set; }
        [JsonProperty("pz")]
        public float pos_z { get; set; }
        [JsonProperty("rx")]
        public float rot_x { get; set; }
        [JsonProperty("ry")]
        public float rot_y { get; set; }
        [JsonProperty("rz")]
        public float rot_z { get; set; }
    }
    public class HQMSkater: HQMObject
    {
        [JsonProperty("spx")]
        public float stick_pos_x { get; set; }
        [JsonProperty("spy")]
        public float stick_pos_y { get; set; }
        [JsonProperty("spz")]
        public float stick_pos_z { get; set; }
        [JsonProperty("srx")]
        public float stick_rot_x { get; set; }
        [JsonProperty("sry")]
        public float stick_rot_y { get; set; }
        [JsonProperty("srz")]
        public float stick_rot_z { get; set; }
        [JsonProperty("bt")]
        public float body_turn { get; set; }
        [JsonProperty("bl")]
        public float body_lean { get; set; }
    }

    public class HQMPuck : HQMObject
    {
       
    }

    public enum HQMMessageType
    {
        PlayerUpdate,
        Goal,
        Chat
    }
    public class HQMMessage
    {
        [JsonProperty("t")]
        public HQMMessageType type { get; set; }
        [JsonProperty("pn")]
        public string player_name { get; set; }
        [JsonProperty("o")]
        public Tuple<int?, HQMTeam?>? objectItem { get; set; }
        [JsonProperty("pi")]
        public int? player_index { get; set; }
        [JsonProperty("is")]
        public bool in_server { get; set; }
        [JsonProperty("tm")]
        public HQMTeam team { get; set; }
        [JsonProperty("gpi")]
        public int? goal_player_index { get; set; }
        [JsonProperty("api")]
        public int? assist_player_index { get; set; }
        [JsonProperty("m")]
        public string message { get; set; }
    }

    public class HQMGameState
    {
        [JsonProperty("pn")]
        public uint packet_number { get; set; }
        [JsonProperty("rs")]
        public uint red_score { get; set; }
        [JsonProperty("bs")]
        public uint blue_score { get; set; }
        [JsonProperty("p")]
        public uint period { get; set; }
        [JsonProperty("t")]
        public uint time { get; set; }
        public List<HQMObject> objects { get; set; }
        public List<HQMMessage> messages_in_this_packet { get; set; }
        [JsonProperty("pl")]
        public List<HQMServerPlayer> player_list { get; set; }
    }

    public class HQMSkaterPacket
    {
        public (uint, uint, uint) pos;
        public (uint, uint) rot;
        public (uint, uint, uint) stick_pos;
        public (uint, uint) stick_rot;
        public uint body_turn;
        public uint body_lean;
    }

    public class HQMPuckPacket
    {
        public (uint, uint, uint) pos;
        public (uint, uint) rot;
    }
}
