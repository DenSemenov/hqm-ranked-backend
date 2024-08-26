using Newtonsoft.Json;
using ReplayHandler.Classes;
using System.Numerics;
using System.Xml.Linq;

namespace ReplayHandler
{
    public static class ReplayHandler
    {
        public static List<ReplayTick> ParseReplay(byte[] data)
        {
            var replayTicks = new List<ReplayTick>();
            var dataLen = data.Length;
            var reader = new HQMMessageReader(data);
            reader.ReadU32Aligned();
            reader.ReadU32Aligned();

            var oldSavedPackets = new Dictionary<uint, object>();
            var currentPlayerList = Enumerable.Repeat<HQMServerPlayer?>(null, 63).ToList();
            uint currentMsgPos = 0;
            var j = 0;
            uint packet = 0;
            var prevMessagesPacket = new List<HQMMessage>();

            while (reader.pos < dataLen)
            {
                reader.ReadByteAligned();
                reader.ReadBits(1) ;
                var redScore = reader.ReadBits(8);
                var blueScore = reader.ReadBits(8);
                var time = reader.ReadBits(16);
                reader.ReadBits(16);
                var period = reader.ReadBits(8);

                var (objects, packetNumber) = ReadObjects(ref reader, ref oldSavedPackets);

                var messageNum = reader.ReadBits(16);
                var msgPos = reader.ReadBits(16);
                var messagesInThisPacket = new List<HQMMessage>();

                for (int i = 0; i < messageNum; i++)
                {
                    var msgPosOfThisMessage = msgPos + i;
                    var msg = ReadMessage(ref reader);

                    if (msgPosOfThisMessage >= currentMsgPos)
                    {
                        ProcessMessage(ref msg, ref currentPlayerList);
                        messagesInThisPacket.Add(msg);
                    }
                }
                currentMsgPos = msgPos + messageNum;
                reader.Next();

                AddReplayTick(replayTicks, packet, (int)redScore, (int)blueScore, (int)period, (int)time, objects, currentPlayerList, messagesInThisPacket, prevMessagesPacket);
                packet++;
                j++;
            }

            return replayTicks;
        }

        private static void AddReplayTick(List<ReplayTick> replayTicks, uint packet, int redScore, int blueScore, int period, int time, List<HQMObject> objects, List<HQMServerPlayer?> currentPlayerList, List<HQMMessage> messagesInThisPacket, List<HQMMessage> prevMessagesPacket)
        {
            var players = currentPlayerList.Where(player => player?.team_and_skater != null)
                                           .Select(player => new PlayerInList
                                           {
                                               Index = (int)player.team_and_skater.Item1,
                                               Team = player.team_and_skater.Item2 == HQMTeam.Red ? ReplayTeam.Red : ReplayTeam.Blue,
                                               Name = player.name,
                                               ListIndex = player.index
                                           }).ToList();

            var replayMessages = messagesInThisPacket.Concat(prevMessagesPacket).Select(message => new ReplayMessage
            {
                Message = message.message,
                AssistIndex = message.assist_player_index,
                GoalIndex = message.goal_player_index,
                InServer = message.in_server,
                ObjectIndex = message.objectItem?.Item1,
                Team = message.team == HQMTeam.Red ? ReplayTeam.Red : ReplayTeam.Blue,
                PlayerIndex = message.player_index,
                PlayerName = message.player_name,
                ReplayMessageType = message.type switch
                {
                    HQMMessageType.PlayerUpdate => ReplayMessageType.PlayerUpdate,
                    HQMMessageType.Chat => ReplayMessageType.Chat,
                    HQMMessageType.Goal => ReplayMessageType.Goal,
                },
                UpdatePlayerIndex = message.player_index
            }).ToList();

            var pucks = objects.OfType<HQMPuck>().Select(puck => new ReplayPuck
            {
                Index = puck.index,
                PosX = Math.Round(puck.pos_x,3),
                PosY = Math.Round(puck.pos_y, 3),
                PosZ = Math.Round(puck.pos_z, 3),
                RotX = Math.Round(puck.rot_x, 3),
                RotY = Math.Round(puck.rot_y, 3),
                RotZ = Math.Round(puck.rot_z, 3)
            }).ToList();

            var skaters = objects.OfType<HQMSkater>().Select(skater => new ReplayPlayer
            {
                Index = skater.index,
                PosX = Math.Round(skater.pos_x, 3),
                PosY = Math.Round(skater.pos_y, 3),
                PosZ = Math.Round(skater.pos_z, 3),
                RotX = Math.Round(skater.rot_x, 3),
                RotY = Math.Round(skater.rot_y, 3),
                RotZ = Math.Round(skater.rot_z, 3),
                StickPosX = Math.Round(skater.stick_pos_x, 3),
                StickPosY = Math.Round(skater.stick_pos_y, 3),
                StickPosZ = Math.Round(skater.stick_pos_z, 3),
                StickRotX = Math.Round(skater.stick_rot_x, 3),
                StickRotY = Math.Round(skater.stick_rot_y, 3),
                StickRotZ = Math.Round(skater.stick_rot_z, 3),
                HeadTurn = Math.Round(skater.body_turn, 3),
                BodyLean = Math.Round(skater.body_lean, 3)
            }).ToList();

            if (skaters.Count > 0)
            {
                replayTicks.Add(new ReplayTick
                {
                    PacketNumber = packet,
                    RedScore = redScore,
                    BlueScore = blueScore,
                    Period = period,
                    Time = time,
                    Pucks = pucks,
                    Players = skaters,
                    PlayersInList = players,
                    Messages = replayMessages
                });
            }
        }

        public static HQMMessage ReadMessage(ref HQMMessageReader reader)
        {
            var messageType = reader.ReadBits(6);
            return messageType switch
            {
                0 => ReadPlayerUpdateMessage(ref reader),
                1 => ReadGoalMessage(ref reader),
                2 => ReadChatMessage(ref reader),
            };
        }

        private static HQMMessage ReadPlayerUpdateMessage(ref HQMMessageReader reader)
        {
            var playerIndex = (int)reader.ReadBits(6);
            var inServer = reader.ReadBits(1) == 1;
            HQMTeam? team = null;
            var teamBits = reader.ReadBits(2);
            if (teamBits == 0)
                team = HQMTeam.Red;
            else if (teamBits == 1)
                team = HQMTeam.Blue;

            var objectIndex = reader.ReadBits(6) is var bits and not 0x3F ? Tuple.Create((int?)bits, team) : null;

            List<byte> bytes = new List<byte>();
            for (int i = 0; i < 31; i++)
            {
                bytes.Add((byte)reader.ReadBits(7));
            }
            var playerName = bytes.TakeWhile(b => b != 0).ToArray();
            return new HQMMessage
            {
                type = HQMMessageType.PlayerUpdate,
                player_name = System.Text.Encoding.UTF8.GetString(playerName).TrimEnd('\0'),
                objectItem = objectIndex,
                player_index = playerIndex,
                in_server = inServer
            };
        }

        private static HQMMessage ReadGoalMessage(ref HQMMessageReader reader)
        {
            var team = reader.ReadBits(2) == 0 ? HQMTeam.Red : HQMTeam.Blue;
            var goalPlayerIndex = (int)reader.ReadBits(6);
            var assistPlayerIndex = (int)reader.ReadBits(6);

            return new HQMMessage
            {
                team = team,
                type = HQMMessageType.Goal,
                goal_player_index = goalPlayerIndex,
                assist_player_index = assistPlayerIndex
            };
        }

        private static HQMMessage ReadChatMessage(ref HQMMessageReader reader)
        {
            var playerIndex = reader.ReadBits(6);
            var size = (int)reader.ReadBits(6);

            var bytes = new List<byte>();
            for (int i = 0; i < size; i++)
            {
                bytes.Add((byte)reader.ReadBits(7));
            }
            var chatMessage = System.Text.Encoding.UTF8.GetString(bytes.ToArray()).TrimEnd('\0');

            return new HQMMessage
            {
                type = HQMMessageType.Chat,
                player_index = playerIndex != 0x3F ? (int)playerIndex : -1,
                message = chatMessage
            };
        }

        private static void ProcessMessage(ref HQMMessage msg, ref List<HQMServerPlayer?> currentPlayerList)
        {
            switch (msg.type)
            {
                case HQMMessageType.PlayerUpdate:
                    UpdatePlayer(msg, ref currentPlayerList);
                    break;
                case HQMMessageType.Goal:
                case HQMMessageType.Chat:
                    UpdateMessagePlayerName(ref msg, currentPlayerList);
                    break;
            }
        }

        private static void UpdatePlayer(HQMMessage msg, ref List<HQMServerPlayer?> currentPlayerList)
        {
            var playerIndex = msg.player_index;
            if (msg.in_server)
            {
                currentPlayerList[(int)playerIndex] = new HQMServerPlayer
                {
                    name = msg.player_name,
                    team_and_skater = msg.objectItem,
                    index = playerIndex
                };
            }
            else
            {
                currentPlayerList[(int)playerIndex] = null;
            }
        }

        private static void UpdateMessagePlayerName(ref HQMMessage msg, List<HQMServerPlayer?> currentPlayerList)
        {
            var playerIndex = msg.player_index ?? msg.goal_player_index;
            if (playerIndex.HasValue && playerIndex.Value != -1)
            {
                var player = currentPlayerList.Where(x=>x !=null).FirstOrDefault(x=>x.index == playerIndex);
                msg.player_name = player?.name;
            }
        }

        public static (List<HQMObject>, uint) ReadObjects(ref HQMMessageReader reader, ref Dictionary<uint, object> history)
        {
            uint current_packet_num = reader.ReadU32Aligned();
            uint previous_packet_num = reader.ReadU32Aligned();
            List<dynamic> find_old = null;
            if (history.ContainsKey(previous_packet_num))
            {
                find_old = (List<dynamic>)history[previous_packet_num];
            }
            List<dynamic> packets = new List<dynamic>();
            for (int i = 0; i < 32; i++)
            {
                bool is_object = reader.ReadBits(1) == 1;
                dynamic? packet = null;
                if (is_object)
                {
                    dynamic? old_object_in_this_slot = find_old?[i];
                    int object_type = (int)reader.ReadBits(2);
                    if (object_type == 0)
                    {
                        HQMSkaterPacket old_skater = old_object_in_this_slot is HQMSkaterPacket ? (HQMSkaterPacket)old_object_in_this_slot : null;
                        var x = reader.ReadPos(17, old_skater?.pos.Item1);
                        var y = reader.ReadPos(17, old_skater?.pos.Item2);
                        var z = reader.ReadPos(17, old_skater?.pos.Item3);

                        var r1 = reader.ReadPos(31, old_skater?.rot.Item1);
                        var r2 = reader.ReadPos(31, old_skater?.rot.Item2);

                        var stick_x = reader.ReadPos(13, old_skater?.stick_pos.Item1);
                        var stick_y = reader.ReadPos(13, old_skater?.stick_pos.Item2);
                        var stick_z = reader.ReadPos(13, old_skater?.stick_pos.Item3);

                        var stick_r1 = reader.ReadPos(25, old_skater?.stick_rot.Item1);
                        var stick_r2 = reader.ReadPos(25, old_skater?.stick_rot.Item2);

                        var body_turn = reader.ReadPos(16, old_skater?.body_turn);
                        var body_lean = reader.ReadPos(16, old_skater?.body_lean);

                        packet = new HQMSkaterPacket
                        {
                            pos = (x, y, z),
                            rot = (r1, r2),
                            stick_pos = (stick_x, stick_y, stick_z),
                            stick_rot = (stick_r1, stick_r2),
                            body_turn = body_turn,
                            body_lean = body_lean
                        };
                    }
                    else if (object_type == 1)
                    {
                        HQMPuckPacket old_puck = old_object_in_this_slot is HQMPuckPacket ? (HQMPuckPacket)old_object_in_this_slot : null;
                        var x = reader.ReadPos(17, old_puck?.pos.Item1);
                        var y = reader.ReadPos(17, old_puck?.pos.Item2);
                        var z = reader.ReadPos(17, old_puck?.pos.Item3);
                        var r1 = reader.ReadPos(31, old_puck?.rot.Item1);
                        var r2 = reader.ReadPos(31, old_puck?.rot.Item2);
                        packet = new HQMPuckPacket
                        {
                            pos = (x, y, z),
                            rot = (r1, r2)
                        };
                    }
                    else
                    {
                        throw new Exception("Unknown object type");
                    }
                }
                packets.Add(packet);
            }
            var objects = new List<HQMObject>();
            int index = 0;
            foreach (var packet in packets)
            {
                switch (packet)
                {
                    case null:
                        break;
                    case HQMPuckPacket puckPacket:
                        float pos_x = puckPacket.pos.Item1 / 1024.0f;
                        float pos_y = puckPacket.pos.Item2 / 1024.0f;
                        float pos_z = puckPacket.pos.Item3 / 1024.0f;
                        var rot = HQMParse.ConvertMatrixFromNetwork(31, puckPacket.rot.Item1, puckPacket.rot.Item2);
                        var rot_x = rot.X;
                        var rot_y = rot.Y;
                        var rot_z = rot.Z;
                        objects.Add(new HQMPuck
                        {
                            index = index,
                            pos_x = 30-pos_x,
                            pos_y = pos_y,
                            pos_z = pos_z,
                            rot_x = rot_x,
                            rot_y = rot_y,
                            rot_z = rot_z
                        });
                        break;
                    case HQMSkaterPacket skaterPacket:
                        float pos_x_p = skaterPacket.pos.Item1 / 1024.0f;
                        float pos_y_p = skaterPacket.pos.Item2 / 1024.0f;
                        float pos_z_p = skaterPacket.pos.Item3 / 1024.0f;
                        float stick_pos_x = (skaterPacket.stick_pos.Item1 / 1024.0f) + pos_x_p - 4.0f;
                        float stick_pos_y = (skaterPacket.stick_pos.Item2 / 1024.0f) + pos_y_p - 4.0f;
                        float stick_pos_z = (skaterPacket.stick_pos.Item3 / 1024.0f) + pos_z_p - 4.0f;
                        var stick_rot_p = HQMParse.ConvertMatrixFromNetwork(31, skaterPacket.stick_rot.Item1, skaterPacket.stick_rot.Item2);
                        var stick_rot_x = stick_rot_p.X;
                        var stick_rot_y = stick_rot_p.Y;
                        var stick_rot_z = stick_rot_p.Z;
                        var rot_p = HQMParse.ConvertMatrixFromNetwork(31, skaterPacket.rot.Item1, skaterPacket.rot.Item2);
                        var rot_x_p = rot_p.X;
                        var rot_y_p = rot_p.Y;
                        var rot_z_p = rot_p.Z;
                        objects.Add(new HQMSkater
                        {
                            index = index,
                            pos_x = 30 - pos_x_p,
                            pos_y = pos_y_p,
                            pos_z = pos_z_p,
                            rot_x = rot_x_p,
                            rot_y = rot_y_p,
                            rot_z = rot_z_p,
                            stick_pos_x = 30 - stick_pos_x,
                            stick_pos_y = stick_pos_y,
                            stick_pos_z = stick_pos_z,
                            stick_rot_x = stick_rot_x,
                            stick_rot_y = stick_rot_y,
                            stick_rot_z = stick_rot_z,
                            body_turn = (skaterPacket.body_turn - 16384.0f) / 8192.0f,
                            body_lean = (skaterPacket.body_lean - 16384.0f) / 8192.0f
                        });
                        break;
                }

                index++;
            }
            history[current_packet_num] = packets;
            return (objects, current_packet_num);
        }
    }
}