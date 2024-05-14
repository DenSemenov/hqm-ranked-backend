using hqm_ranked_backend.Models.DTO;
using Microsoft.AspNetCore.SignalR.Protocol;
using ReplayHandler.Classes;

namespace hqm_ranked_backend.Helpers
{
    public static class ReplayDataHelper
    {

        public static ReplayCalculatedData GetReplayCalcData(List<ReplayTick> ticks)
        {
            var result = new ReplayCalculatedData();

            result.Chats = ticks.SelectMany(x => x.Messages.Select(m => new { Packet = x.PacketNumber, Message = m })).Where(x => x.Message.ReplayMessageType == ReplayMessageType.Chat).Select(x => new ReplayCalculatedChat
            {
                Text = (!String.IsNullOrEmpty(x.Message.PlayerName) ? x.Message.PlayerName +": " : String.Empty) +x.Message.Message,
                Packet = x.Packet
            }).ToList();

            result.Goals = ticks.SelectMany(x => x.Messages.Select(m => new { Packet = x.PacketNumber, Message = m, Period = x.Period, Time = x.Time })).Where(x => x.Message.ReplayMessageType == ReplayMessageType.Goal).Select(x => new ReplayCalculatedGoal
            {
                GoalBy = x.Message.PlayerName,
                Packet = x.Packet,
                Period = x.Period,
                Time = x.Time
            }).ToList();

            return result;
        }
    }
}
