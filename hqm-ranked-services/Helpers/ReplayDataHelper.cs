

using Example.Classes;
using hqm_ranked_backend.Models.DTO;
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
                Text = (!String.IsNullOrEmpty(x.Message.PlayerName) ? x.Message.PlayerName + ": " : String.Empty) + x.Message.Message,
                Packet = x.Packet
            }).ToList();

            result.Goals = ticks.SelectMany(x => x.Messages.Select(m => new { Packet = x.PacketNumber, Message = m, Period = x.Period, Time = x.Time })).Where(x => x.Message.ReplayMessageType == ReplayMessageType.Goal).Select(x => new ReplayCalculatedGoal
            {
                GoalBy = x.Message.PlayerName,
                Packet = x.Packet,
                Period = x.Period,
                Time = x.Time
            }).ToList();

            var withVectors = new List<ReplayTickWithVectors>();

            ReplayTick? prevTick = null;
            int prevTime = ticks.FirstOrDefault().Time;
            int prevPeriod = ticks.FirstOrDefault().Period;
            uint? pauseStartPacket = null;

            foreach (var tick in ticks)
            {
                var packet = new ReplayTickWithVectors();
                packet.Packet = tick.PacketNumber;

                foreach (var player in tick.Players)
                {
                    ReplayPlayer? oldObject = null;

                    if (prevTick != null)
                    {
                        oldObject = prevTick.Players.FirstOrDefault(x => x.Index == player.Index);
                    }

                    var pos = new Vector(player.PosX, player.PosY, player.PosZ);
                    var rot = new Vector(player.RotX, player.RotY, player.RotZ);
                    var stickPos = new Vector(player.StickPosX, player.StickPosY, player.StickPosZ);
                    var stickRot = new Vector(player.StickRotX, player.StickRotY, player.StickRotZ);

                    packet.Objects.Add(new ObjectWithVectors
                    {
                        Type = ObjectType.Player,
                        Index = player.Index,
                        Pos = pos,
                        Rot = rot,
                        StickPos = stickPos,
                        StickRot = stickRot,
                        PosVelocity = Vector.CalcVector(oldObject != null ? new Vector(oldObject.PosX, oldObject.PosY, oldObject.PosZ) : null, pos),
                        RotVelocity = Vector.CalcVector(oldObject != null ? new Vector(oldObject.RotX, oldObject.RotY, oldObject.RotZ) : null, rot),
                        StickPosVelocity = Vector.CalcVector(oldObject != null ? new Vector(oldObject.StickPosX, oldObject.StickPosY, oldObject.StickPosZ) : null, stickPos),
                        StickRotVelocity = Vector.CalcVector(oldObject != null ? new Vector(oldObject.StickRotX, oldObject.StickRotY, oldObject.StickRotZ) : null, stickRot),
                    });
                }

                foreach (var puck in tick.Pucks)
                {
                    ReplayPuck? oldObject = null;

                    if (prevTick != null)
                    {
                        oldObject = prevTick.Pucks.FirstOrDefault(x => x.Index == puck.Index);
                    }
                    var pos = new Vector(puck.PosX, puck.PosY, puck.PosZ);
                    var rot = new Vector(puck.RotX, puck.RotY, puck.RotZ);

                    int? lastTouched = null;

                    foreach (var player in packet.Objects.Where(x => x.Type == ObjectType.Player))
                    {
                        var p = player.StickPos.Subtract(pos);
                        var m = p.Magnitude();
                        if (m < 0.25)
                        {
                            lastTouched = player.Index;
                        }
                    }

                    packet.Objects.Add(new ObjectWithVectors
                    {
                        Type = ObjectType.Puck,
                        Index = puck.Index,
                        Pos = pos,
                        Rot = rot,
                        PosVelocity = Vector.CalcVector(oldObject != null ? new Vector(oldObject.PosX, oldObject.PosY, oldObject.PosZ) : null, pos),
                        RotVelocity = Vector.CalcVector(oldObject != null ? new Vector(oldObject.RotX, oldObject.RotY, oldObject.RotZ) : null, rot),
                        TouchedBy = lastTouched
                    });
                }

                withVectors.Add(packet);

                if (pauseStartPacket == null && prevPeriod == tick.Period && prevTime == tick.Time)
                {
                    pauseStartPacket = tick.PacketNumber;
                }
                else if (pauseStartPacket != null && (prevPeriod != tick.Period || prevTime != tick.Time))
                {
                    result.Pauses.Add(new ReplayCalculatedPause
                    {
                        StartPacket = (uint)pauseStartPacket,
                        EndPacket = tick.PacketNumber,
                    });
                    pauseStartPacket = null;
                }

                prevPeriod = tick.Period;
                prevTime = tick.Time;

                prevTick = tick;

                Thread.Sleep(2);
            }

            foreach (var pause in result.Pauses)
            {
                var redGoaliePos = new Vector(15, 0, 56);
                var blueGoaliePos = new Vector(15, 0, 5);

                var tickAfterPause = ticks.FirstOrDefault(x => x.PacketNumber == pause.EndPacket);

                ReplayPlayer? redGoalie = null;
                ReplayPlayer? blueGoalie = null;

                for (uint i = pause.EndPacket - 5; i < pause.EndPacket + 5; i++)
                {
                    var redG = ticks.FirstOrDefault(x => x.PacketNumber == i).Players.FirstOrDefault(x => x.PosX > redGoaliePos.X - 1 && x.PosX < redGoaliePos.X + 1 && x.PosZ > redGoaliePos.Z - 1 && x.PosZ < redGoaliePos.Z + 1);
                    var blueG = ticks.FirstOrDefault(x => x.PacketNumber == i).Players.FirstOrDefault(x => x.PosX > blueGoaliePos.X - 1 && x.PosX < blueGoaliePos.X + 1 && x.PosZ > blueGoaliePos.Z - 1 && x.PosZ < blueGoaliePos.Z + 1);


                    if (redGoalie == null)
                    {
                        redGoalie = redG;
                    }

                    if (blueGoalie == null)
                    {
                        blueGoalie = blueG;
                    }
                }

                if (tickAfterPause != null)
                {
                    if (redGoalie != null)
                    {
                        var foundPlayer = tickAfterPause.PlayersInList.FirstOrDefault(x => x.Index == redGoalie.Index);
                        if (foundPlayer != null)
                        {
                            if (result.Goalies.Any(x => x.Team == ReplayTeam.Red))
                            {
                                var lastGoalie = result.Goalies.Where(x => x.Team == ReplayTeam.Red).LastOrDefault();
                                if (lastGoalie.Name != foundPlayer.Name)
                                {
                                    lastGoalie.EndPacket = pause.EndPacket - 1;

                                    result.Goalies.Add(new ReplayCalculatedGoaliePosition
                                    {
                                        Name = foundPlayer.Name,
                                        StartPacket = pause.EndPacket,
                                        Team = ReplayTeam.Red,
                                        EndPacket = ticks.LastOrDefault().PacketNumber
                                    });
                                }
                            }
                            else
                            {
                                result.Goalies.Add(new ReplayCalculatedGoaliePosition
                                {
                                    Name = foundPlayer.Name,
                                    StartPacket = pause.EndPacket,
                                    Team = ReplayTeam.Red,
                                    EndPacket = ticks.LastOrDefault().PacketNumber
                                });
                            }
                        }

                    }

                    if (blueGoalie != null)
                    {
                        var foundPlayer = tickAfterPause.PlayersInList.FirstOrDefault(x => x.Index == blueGoalie.Index);
                        if (foundPlayer != null)
                        {
                            if (result.Goalies.Any(x => x.Team == ReplayTeam.Blue))
                            {
                                var lastGoalie = result.Goalies.Where(x => x.Team == ReplayTeam.Blue).LastOrDefault();
                                if (lastGoalie.Name != foundPlayer.Name)
                                {
                                    lastGoalie.EndPacket = pause.EndPacket - 1;

                                    result.Goalies.Add(new ReplayCalculatedGoaliePosition
                                    {
                                        Name = foundPlayer.Name,
                                        StartPacket = pause.EndPacket,
                                        Team = ReplayTeam.Blue,
                                        EndPacket = ticks.LastOrDefault().PacketNumber
                                    });
                                }
                            }
                            else
                            {
                                result.Goalies.Add(new ReplayCalculatedGoaliePosition
                                {
                                    Name = foundPlayer.Name,
                                    StartPacket = pause.EndPacket,
                                    Team = ReplayTeam.Blue,
                                    EndPacket = ticks.LastOrDefault().PacketNumber
                                });
                            }
                        }

                    }
                }
            }

            int? prevTouched = null;
            ReplayTeam? shotDetected = null;

            var index = 0;
            foreach (var tick in withVectors)
            {
                var isPaused = result.Pauses.Any(x => x.StartPacket < tick.Packet && x.EndPacket > tick.Packet);
                if (isPaused)
                {
                    shotDetected = null;
                    prevTouched = null;
                }

                if (tick.Objects.Where(x => x.Type == ObjectType.Puck).Count() == 1 && !isPaused)
                {
                    var currentPacketData = ticks.FirstOrDefault(x => x.PacketNumber == tick.Packet);

                    var puck = tick.Objects.FirstOrDefault(x => x.Type == ObjectType.Puck);
                    var lastTouchedBy = puck.TouchedBy ?? prevTouched;

                    if (puck.TouchedBy != null && shotDetected != null)
                    {
                        if (puck.TouchedBy != prevTouched)
                        {
                            var foundPlayer = currentPacketData.PlayersInList.FirstOrDefault(x => x.Index == lastTouchedBy);
                            if (foundPlayer != null)
                            {
                                if (foundPlayer.Team != shotDetected)
                                {
                                    var goalFound = false;
                                    for (var i = tick.Packet; i <= tick.Packet + 300; i++)
                                    {
                                        var nextTick = withVectors.FirstOrDefault(x => x.Packet == i);
                                        if (nextTick != null)
                                        {
                                            var goalInNextTick = result.Goals.FirstOrDefault(x => x.Packet == nextTick.Packet);
                                            if (goalInNextTick != null)
                                            {
                                                goalFound = true;
                                            }
                                        }
                                    }

                                    if (!goalFound)
                                    {
                                        result.Saves.Add(new ReplayCalculatedSave
                                        {
                                            Name = foundPlayer.Name,
                                            Packet = tick.Packet
                                        });
                                    }
                                }
                            }

                            shotDetected = null;
                        }
                    }

                    if (lastTouchedBy != null)
                    {
                        if (currentPacketData != null)
                        {
                            var foundPlayer = currentPacketData.PlayersInList.FirstOrDefault(x => x.Index == lastTouchedBy);
                            if (foundPlayer != null)
                            {
                                var playerInPossession = result.Possession.FirstOrDefault(x => x.Name == foundPlayer.Name);
                                if (playerInPossession != null)
                                {
                                    playerInPossession.Touches += 1;
                                }
                                else
                                {
                                    result.Possession.Add(new ReplayCalculatedPossession
                                    {
                                        Name = foundPlayer.Name,
                                        Touches = 1
                                    });
                                }

                                //shot counter
                                if (shotDetected == null)
                                {
                                    double t = 0;
                                    double x = 0;
                                    double y = 0;

                                    var puckGoingTo = puck.PosVelocity.Z < 0 ? ReplayTeam.Red : ReplayTeam.Blue;

                                    if (puckGoingTo == ReplayTeam.Red)
                                    {
                                        t = (4.15 - puck.Pos.Z) / puck.PosVelocity.Z;
                                    }
                                    else
                                    {
                                        t = (56.85 - puck.Pos.Z) / puck.PosVelocity.Z;
                                    }

                                    x = puck.Pos.X + puck.PosVelocity.X * t;
                                    y = puck.Pos.Y + puck.PosVelocity.Y * t;

                                    if (x > 13.75 && x < 16.25 && puckGoingTo == ReplayTeam.Red)
                                    {
                                        if (y < .83)
                                        {
                                            if (puck.Pos.Z < 10 && puck.Pos.Z > 3.8 && puck.Pos.X < 19 && puck.Pos.X > 11)
                                            {
                                                if (foundPlayer.Team == ReplayTeam.Red)
                                                {
                                                    var packetNumber = tick.Packet;
                                                    if (puck.TouchedBy == null)
                                                    {
                                                        for (int i = index + 1; i >= 0; i--)
                                                        {
                                                            var oldPuck = withVectors[i].Objects.FirstOrDefault(x => x.Type == ObjectType.Puck);
                                                            if (oldPuck != null)
                                                            {
                                                                if (oldPuck.TouchedBy == lastTouchedBy)
                                                                {
                                                                    packetNumber = (uint)i;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    result.Shots.Add(new ReplayCalculatedShot
                                                    {
                                                        Name = foundPlayer.Name,
                                                        Packet = packetNumber,
                                                    });
                                                    shotDetected = ReplayTeam.Red;
                                                }
                                            }
                                        }
                                    }
                                    else if (x > 13.75 && x < 16.25 && puckGoingTo == ReplayTeam.Blue)
                                    {
                                        if (y < .83)
                                        {
                                            if (puck.Pos.Z > 51 && puck.Pos.Z < 57.2 && puck.Pos.X < 19 && puck.Pos.X > 11)
                                            {
                                                if (foundPlayer.Team == ReplayTeam.Blue)
                                                {
                                                    var packetNumber = tick.Packet;
                                                    if (puck.TouchedBy == null)
                                                    {
                                                        for (int i = index + 1; i >= 0; i--)
                                                        {
                                                            var oldPuck = withVectors[i].Objects.FirstOrDefault(x => x.Type == ObjectType.Puck);
                                                            if (oldPuck != null)
                                                            {
                                                                if (oldPuck.TouchedBy == lastTouchedBy)
                                                                {
                                                                    packetNumber = (uint)i;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    result.Shots.Add(new ReplayCalculatedShot
                                                    {
                                                        Name = foundPlayer.Name,
                                                        Packet = packetNumber,
                                                    });
                                                    shotDetected = ReplayTeam.Blue;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        prevTouched = lastTouchedBy;
                    }
                }

                index += 1;

                Thread.Sleep(2);
            }

            return result;
        }
    }
}
