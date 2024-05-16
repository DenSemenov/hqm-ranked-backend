using Newtonsoft.Json;
using ReplayHandler.Classes;
using System.Numerics;

namespace ReplayHandler
{
    public static class ReplayHandler
    {
        public static List<ReplayTick> ParseReplay(byte[] data)
        {
            var replayTicks = new List<ReplayTick>();
            
            var data_len = data.Length;
            var reader = new HQMMessageReader(data);
            reader.ReadU32Aligned();
            var _bytes = reader.ReadU32Aligned();

            var old_saved_packets = new Dictionary<uint, object>();

            var current_player_list = new List<HQMServerPlayer?>();
            for (int i = 0; i < 63; i++)
            {
                current_player_list.Add(null);
            }

            uint current_msg_pos = 0;

            var j = 0;
            uint packet = 0;

            var prevMessagesPacket = new List<HQMMessage>();

            while (reader.pos < data_len)
            {
                reader.ReadByteAligned(); // Should be 5, but we're not checking
                var game_over = reader.ReadBits(1) == 1;
                var red_score = reader.ReadBits(8);
                var blue_score = reader.ReadBits(8);
                var time = reader.ReadBits(16);
                var goal_message_timer = reader.ReadBits(16);
                var period = reader.ReadBits(8);

                var (objects, packet_number) = read_objects(ref reader, ref old_saved_packets);

                var message_num = reader.ReadBits(16);
                var msg_pos = reader.ReadBits(16);
                var messages_in_this_packet = new List<HQMMessage>();
                for (int i = 0; i < message_num; i++)
                {
                    var msg_pos_of_this_message = msg_pos + i;
                    var msg = ReadMessage(ref reader);

                    if (msg_pos_of_this_message >= current_msg_pos)
                    {
                        switch (msg.type)
                        {
                            case HQMMessageType.PlayerUpdate:
                                {
                                    var player_name = msg.player_name;
                                    var objectItem = msg.objectItem;
                                    var player_index = msg.player_index;
                                    var in_server = msg.in_server;

                                    if (in_server)
                                    {
                                        current_player_list[(int)player_index] = new HQMServerPlayer
                                        {
                                            
                                            name = player_name,
                                            team_and_skater = objectItem,
                                            index = player_index
                                        };
                                    }
                                    else
                                    {
                                        current_player_list[(int)player_index] = null;
                                    }
                                    break;
                                }
                            case HQMMessageType.Goal:
                                {
                                    var player_index = msg.goal_player_index;

                                    string name = null;
                                    if (player_index.HasValue)
                                    {
                                        if (player_index.Value != -1)
                                        {
                                            try
                                            {
                                                var p = current_player_list[player_index.Value];
                                                name = p?.name;
                                                msg.player_name = name;
                                            }
                                            catch { }
                                        }
                                    }
                                    break;
                                }
                            case HQMMessageType.Chat:
                                {
                                    var player_index = msg.player_index;
                                    var message = msg.message;

                                    string name = null;
                                    if (player_index.HasValue)
                                    {
                                        if (player_index.Value != -1)
                                        {
                                            var p = current_player_list[player_index.Value];
                                            name = p?.name;
                                            msg.player_name = name;
                                        }
                                    }

                                    break;
                                }
                        }

                        messages_in_this_packet.Add(msg);
                    }
                }
                current_msg_pos = msg_pos + message_num;

                reader.Next();

                if (j % 2 == 0)
                {
                    var pucks = new List<ReplayPuck>();
                    var players = new List<ReplayPlayer>();
                    var playersInList = new List<PlayerInList>();

                    foreach (var player in current_player_list.Where(x => x != null).Where(x => x != null))
                    {
                        if (player.team_and_skater != null)
                        {
                            var index = (int)player.team_and_skater.Item1;
                            var team = player.team_and_skater.Item2 == HQMTeam.Red ? ReplayTeam.Red : ReplayTeam.Blue;
                            playersInList.Add(new PlayerInList
                            {
                                ListIndex = player.index,
                                Name = player.name,
                                Index = index,
                                Team = team,
                            });
                        }
                    }

                    foreach (HQMObject obj in objects)
                    {
                        if (obj is HQMSkater)
                        {
                            var pl = obj as HQMSkater;

                            players.Add(new ReplayPlayer
                            {
                                Index = pl.index,
                                PosX = pl.pos_x,
                                PosY = pl.pos_y,
                                PosZ = pl.pos_z,
                                RotX = pl.rot_x,
                                RotY = pl.rot_y,
                                RotZ = pl.rot_z,
                                StickPosX = pl.stick_pos_x,
                                StickPosY = pl.stick_pos_y,
                                StickPosZ = pl.stick_pos_z,
                                StickRotX = pl.stick_rot_x,
                                StickRotY = pl.stick_rot_y,
                                StickRotZ = pl.stick_rot_z,
                                HeadTurn = pl.body_turn,
                                BodyLean = pl.body_lean
                            });


                        }
                        else
                        {
                            var puck = obj as HQMPuck;
                            pucks.Add(new ReplayPuck
                            {
                                Index = puck.index,
                                PosX = puck.pos_x,
                                PosY = puck.pos_y,
                                PosZ = puck.pos_z,
                                RotX = puck.rot_x,
                                RotY = puck.rot_y,
                                RotZ = puck.rot_z,
                            });
                        }
                    }

                    var messages = messages_in_this_packet;
                    messages.AddRange(prevMessagesPacket);

                    var replayMessages = new List<ReplayMessage>();

                    foreach(var message in messages)
                    {
                        var type = ReplayMessageType.PlayerUpdate;
                        switch(message.type)
                        {
                            case HQMMessageType.PlayerUpdate:
                                type = ReplayMessageType.PlayerUpdate;
                                break;
                            case HQMMessageType.Chat:
                                type = ReplayMessageType.Chat;
                                break;
                            case HQMMessageType.Goal:
                                type = ReplayMessageType.Goal;
                                break;
                        }

                        var team = ReplayTeam.Spectator;
                        switch (message.team)
                        {
                            case HQMTeam.Red:
                                team = ReplayTeam.Red;
                                break;
                            case HQMTeam.Blue:
                                team = ReplayTeam.Blue;
                                break;
                        }

                        replayMessages.Add(new ReplayMessage
                        {
                            Message = message.message,
                            AssistIndex = message.assist_player_index,
                            GoalIndex = message.goal_player_index,
                            InServer = message.in_server,
                            ObjectIndex = message.objectItem?.Item1,
                            Team = team,
                            PlayerIndex = message.player_index,
                            PlayerName = message.player_name,
                            ReplayMessageType = type,
                            UpdatePlayerIndex = message.player_index
                        });
                    }

                    if (players.Count != 0)
                    {
                        var replayTick = new ReplayTick
                        {
                            PacketNumber = packet,
                            RedScore = (int)red_score,
                            BlueScore = (int)blue_score,
                            Period = (int)period,
                            Time = (int)time,
                            Pucks = pucks,
                            Players = players,
                            PlayersInList = playersInList,
                            Messages = replayMessages
                        };

                        replayTicks.Add(replayTick);

                        packet += 1;
                    }
                }
                else
                {
                    prevMessagesPacket = messages_in_this_packet;
                }

                j += 1;
            }

            return replayTicks;
        }

        public static HQMMessage ReadMessage(ref HQMMessageReader reader)
        {
            var messageType = reader.ReadBits(6);

            if (messageType == 0)
            {
                var playerIndex = (int)reader.ReadBits(6);
                bool inServer = reader.ReadBits(1) == 1;

                HQMTeam? team = null;
                var teamBits = reader.ReadBits(2);
                if (teamBits == 0)
                    team = HQMTeam.Red;
                else if (teamBits == 1)
                    team = HQMTeam.Blue;

                Tuple<int?, HQMTeam?>? objectIndex = null;
                var objectBits = reader.ReadBits(6);
                if (objectBits != 0x3F)
                    objectIndex = Tuple.Create((int?)objectBits, team);

                List<byte> bytes = new List<byte>();
                for (int i = 0; i < 31; i++)
                {
                    bytes.Add((byte)reader.ReadBits(7));
                }

                string playerName = null;
                if (bytes.Count > 0)
                {
                    var indexEmpty = bytes.IndexOf(bytes.FirstOrDefault(x => x == 0));
                    var b = bytes.Where((v, index) => index < indexEmpty);
                    string s = System.Text.Encoding.UTF8.GetString(b.ToArray());
                    playerName = s.TrimEnd('\0');
                }

                return new HQMMessage
                {
                    type = HQMMessageType.PlayerUpdate,
                    player_name = playerName,
                    objectItem = objectIndex,
                    player_index = playerIndex,
                    in_server = inServer
                };
            }
            else if (messageType == 1)
            {
                HQMTeam team = HQMTeam.Red;
                int teamBits = (int)reader.ReadBits(2);
                if (teamBits == 0)
                    team = HQMTeam.Red;
                else
                    team = HQMTeam.Blue;

                int goalPlayerIndex = (int)reader.ReadBits(6);
                int assistPlayerIndex = (int)reader.ReadBits(6);

                return new HQMMessage
                {
                    team = team,
                    type = HQMMessageType.Goal,
                    goal_player_index = goalPlayerIndex,
                    assist_player_index = assistPlayerIndex
                };
            }
            else if (messageType == 2)
            {
                var playerIndex = reader.ReadBits(6);
                int size = (int)reader.ReadBits(6);

                List<byte> bytes = new List<byte>();
                for (int i = 0; i < size; i++)
                {
                    bytes.Add((byte)reader.ReadBits(7));
                }

                string chatMessage = null;
                if (bytes.Count > 0)
                {
                    string s = System.Text.Encoding.UTF8.GetString(bytes.ToArray());
                    chatMessage = s.TrimEnd('\0');
                }

                return new HQMMessage
                {
                    type = HQMMessageType.Chat,
                    player_index = playerIndex != 0x3F ? (int)playerIndex : -1,
                    message = chatMessage
                };
            }
            else
            {
                throw new Exception("Unknown message type");
            }
        }

        public static (List<HQMObject>, uint) read_objects(ref HQMMessageReader reader, ref Dictionary<uint, object> history)
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
                        float rot_x = rot.x;
                        float rot_y = rot.y;
                        float rot_z = rot.z;
                        objects.Add(new HQMPuck
                        {
                            index = index,
                            pos_x = pos_x,
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
                        float stick_rot_x = stick_rot_p.x;
                        float stick_rot_y = stick_rot_p.y;
                        float stick_rot_z = stick_rot_p.z;
                        var rot_p = HQMParse.ConvertMatrixFromNetwork(31, skaterPacket.rot.Item1, skaterPacket.rot.Item2);
                        float rot_x_p = rot_p.x;
                        float rot_y_p = rot_p.y;
                        float rot_z_p = rot_p.z;
                        objects.Add(new HQMSkater
                        {
                            index = index,
                            pos_x = pos_x_p,
                            pos_y = pos_y_p,
                            pos_z = pos_z_p,
                            rot_x = rot_x_p,
                            rot_y = rot_y_p,
                            rot_z = rot_z_p,
                            stick_pos_x = stick_pos_x,
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