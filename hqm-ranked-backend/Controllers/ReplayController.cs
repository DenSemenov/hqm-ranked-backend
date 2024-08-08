using hqm_ranked_backend.Common;
using hqm_ranked_backend.Helpers;
using hqm_ranked_backend.Models.InputModels;
using hqm_ranked_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReplayHandler.Classes;
using Serilog;

namespace hqm_ranked_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReplayController : ControllerBase
    {
        IReplayService _replayService;
        IReplayCalcService _replayCalcService;
        public ReplayController(IReplayService replayService, IReplayCalcService replayCalcService)
        {
            _replayService = replayService;
            _replayCalcService = replayCalcService;
        }

        [HttpPost("ProcessHrpLocal")]
        public async Task<IActionResult> ProcessHrpLocal([FromForm] IFormFile file)
        {

            long length = file.Length;
            if (length < 0)
                return BadRequest();

            using var fileStream = file.OpenReadStream();
            byte[] bytes = new byte[length];
            fileStream.Read(bytes, 0, (int)file.Length);

            var result = await _replayCalcService.ProcessReplay(bytes);

            GetScript(new ReplayClass
            {
                Ticks = result
            });

            return Ok(result);
        }

        private void GetScript(ReplayClass replayClass)
        {
            using (StreamWriter writetext = new StreamWriter("D://test.txt"))
            {
                writetext.WriteLine("import bpy");
                writetext.WriteLine("from mathutils import Vector");
                writetext.WriteLine("scene = bpy.context.scene");
                writetext.WriteLine("scene.objects.keys()");
                writetext.WriteLine("baseredlower = bpy.data.objects[\"baseredlower\"]");
                writetext.WriteLine("baseredupper = bpy.data.objects[\"baseredupper\"]");
                writetext.WriteLine("basebluelower = bpy.data.objects[\"baseredlower\"]");
                writetext.WriteLine("baseblueupper = bpy.data.objects[\"baseredupper\"]");
                writetext.WriteLine("basepuck = bpy.data.objects[\"basepuck\"]");
                writetext.WriteLine("basestick = bpy.data.objects[\"basestick\"]");
                writetext.WriteLine("baseredgoal = bpy.data.objects[\"baseredgoal\"]");
                writetext.WriteLine("basebluegoal = bpy.data.objects[\"basebluegoal\"]");
                writetext.WriteLine("basetime = bpy.data.objects[\"basetime\"]");
                writetext.WriteLine("baseperiod = bpy.data.objects[\"baseperiod\"]");
                writetext.WriteLine("baseredscore = bpy.data.objects[\"baseredscore\"]");
                writetext.WriteLine("basebluescore = bpy.data.objects[\"basebluescore\"]");
                writetext.WriteLine("basegoalby = bpy.data.objects[\"basegoalby\"]");
                writetext.WriteLine("basegoalbytitle = bpy.data.objects[\"basegoalby\"]");
                writetext.WriteLine("baseassistby = bpy.data.objects[\"baseassistby\"]");
                writetext.WriteLine("baseassistbytitle = bpy.data.objects[\"baseassistby\"]");
                writetext.WriteLine("basesoundtitle = bpy.data.objects[\"basesoundtitle\"]");
                writetext.WriteLine("redmaterial = bpy.data.materials.new(name=\"redm\")");
                writetext.WriteLine("redmaterial.use_nodes = True");
                writetext.WriteLine("redmaterial.node_tree.nodes[\"Principled BSDF\"].inputs[0].default_value= (0.799103, 0, 0.0139229, 1)");
                writetext.WriteLine("bluematerial = bpy.data.materials.new(name=\"bluem\")");
                writetext.WriteLine("bluematerial.use_nodes = True");
                writetext.WriteLine("bluematerial.node_tree.nodes[\"Principled BSDF\"].inputs[0].default_value=(0.00282698, 0, 0.8, 1)");

                var timeArray = replayClass.Ticks.Where((x, index) => index % 4 == 0).Select(x => "\"" + TimeSpan.FromSeconds(x.Time / 100).ToString(@"m\:ss") + "\"").ToList();
                timeArray.AddRange(timeArray);
                writetext.WriteLine("timearray=[" + String.Join(",", timeArray) + "]");

                writetext.WriteLine("def set_time(scene):");
                writetext.WriteLine("    basetime.data.body=str(timearray[scene.frame_current])");
                writetext.WriteLine("bpy.app.handlers.frame_change_pre.append(set_time)");

                var periodArray = replayClass.Ticks.Where((x, index) => index % 4 == 0).Select(x => "\"" + "1" + "\"").ToList();
                periodArray.AddRange(periodArray);
                writetext.WriteLine("periodarray=[" + String.Join(",", periodArray) + "]");

                writetext.WriteLine("def set_period(scene):");
                writetext.WriteLine("    baseperiod.data.body=str(periodarray[scene.frame_current])");
                writetext.WriteLine("bpy.app.handlers.frame_change_pre.append(set_period)");

                var redscoreArray = replayClass.Ticks.Where((x, index) => index % 4 == 0).Select(x => x.RedScore).ToList();
                redscoreArray.AddRange(redscoreArray);
                writetext.WriteLine("redscorearray=[" + String.Join(",", redscoreArray) + "]");

                writetext.WriteLine("def set_redscore(scene):");
                writetext.WriteLine("    baseredscore.data.body=str(redscorearray[scene.frame_current])");
                writetext.WriteLine("bpy.app.handlers.frame_change_pre.append(set_redscore)");

                var bluescoreArray = replayClass.Ticks.Where((x, index) => index % 4 == 0).Select(x => x.BlueScore).ToList();
                bluescoreArray.AddRange(bluescoreArray);
                writetext.WriteLine("bluescorearray=[" + String.Join(",", bluescoreArray) + "]");

                writetext.WriteLine("def set_bluescore(scene):");
                writetext.WriteLine("    basebluescore.data.body=str(bluescorearray[scene.frame_current])");
                writetext.WriteLine("bpy.app.handlers.frame_change_pre.append(set_bluescore)");

                var moving = String.Empty;
                var frame = 1;

                var playerIndexes = new List<int>();

                var index = 1;

                decimal cameraTickStep = (decimal)10 / (decimal)replayClass.Ticks.Count;
                decimal cameraOffset = 20;

                foreach (var tick in replayClass.Ticks)
                {
                    if (index > 12)
                    {
                        if (index % 4 == 0)
                        {
                            writetext.WriteLine("scene.frame_set(" + frame.ToString() + ")");


                            foreach (var player in tick.Players)
                            {
                                var playerInServer = tick.PlayersInList.SingleOrDefault(x => x.Index == player.Index);
                                var team = playerInServer != null ? playerInServer.Team : ReplayTeam.Red;

                                var teamString = team == ReplayTeam.Red ? "red" : "blue";

                                if (!playerIndexes.Contains(player.Index))
                                {
                                    writetext.WriteLine("bpy.ops.object.select_all(action='DESELECT')");

                                    writetext.WriteLine("base" + teamString + "upper.select_set(True) ");
                                    writetext.WriteLine("bpy.ops.object.duplicate()");
                                    writetext.WriteLine("upper" + player.Index + "=bpy.context.selected_objects[0]");
                                    writetext.WriteLine("upper" + player.Index + ".name=\"upper" + player.Index + "\"");

                                    writetext.WriteLine("bpy.ops.object.select_all(action='DESELECT')");
                                    writetext.WriteLine("base" + teamString + "lower.select_set(True) ");
                                    writetext.WriteLine("bpy.ops.object.duplicate()");
                                    writetext.WriteLine("lower" + player.Index + "=bpy.context.selected_objects[0]");
                                    writetext.WriteLine("lower" + player.Index + ".name=\"lower" + player.Index + "\"");

                                    writetext.WriteLine("bpy.ops.object.select_all(action='DESELECT')");


                                    writetext.WriteLine("bpy.ops.object.select_all(action='DESELECT')");
                                    writetext.WriteLine("basestick.select_set(True)");
                                    writetext.WriteLine("bpy.ops.object.duplicate()");
                                    writetext.WriteLine("stick" + player.Index + "=bpy.context.selected_objects[0]");
                                    writetext.WriteLine("stick" + player.Index + ".name=\"stick" + player.Index + "\"");

                                    playerIndexes.Add(player.Index);
                                }
                                var changeupper = "changeupper" + player.Index.ToString() + frame.ToString();

                                writetext.WriteLine(changeupper + "=bpy.data.objects[\"upper" + player.Index + "\"]");
                                writetext.WriteLine(String.Format(changeupper + ".location=({0},{1},{2})", (decimal)player.PosX, (decimal)player.PosZ, (decimal)player.PosY));
                                writetext.WriteLine(changeupper + ".keyframe_insert(data_path=\"location\", index=-1)");

                                var changelower = "changelower" + player.Index.ToString() + frame.ToString();

                                writetext.WriteLine(changelower + "=bpy.data.objects[\"lower" + player.Index + "\"]");
                                writetext.WriteLine(String.Format(changelower + ".location=({0},{1},{2})", (decimal)player.PosX, (decimal)player.PosZ, (decimal)player.PosY));
                                writetext.WriteLine(changelower + ".keyframe_insert(data_path=\"location\", index=-1)");


                                writetext.WriteLine(String.Format(changelower + ".rotation_euler=({0}, {1}, {2})", player.RotX, player.RotY, player.RotZ));
                                writetext.WriteLine(changelower + ".keyframe_insert(data_path=\"rotation_euler\", index=-1)");

                                writetext.WriteLine(String.Format(changeupper + ".rotation_euler=({0}, {1}, {2})", player.RotX - player.BodyLean, player.RotY - player.HeadTurn, player.RotZ));
                                writetext.WriteLine(changeupper + ".keyframe_insert(data_path=\"rotation_euler\", index=-1)");

                                writetext.WriteLine("changestick" + player.Index.ToString() + frame.ToString() + "=bpy.data.objects[\"stick" + player.Index + "\"]");
                                writetext.WriteLine(String.Format("changestick" + player.Index.ToString() + frame.ToString() + ".location=({0},{1},{2})", (decimal)player.StickPosX, (decimal)player.StickPosZ, (decimal)player.StickPosY));
                                writetext.WriteLine("changestick" + player.Index.ToString() + frame.ToString() + ".keyframe_insert(data_path=\"location\", index=-1)");

                                writetext.WriteLine(String.Format("changestick" + player.Index.ToString() + frame.ToString() + ".rotation_euler=({0}, {1}, {2})", player.StickRotX, player.StickRotY, player.StickRotZ));
                                writetext.WriteLine("changestick" + player.Index.ToString() + frame.ToString() + ".keyframe_insert(data_path=\"rotation_euler\", index=-1)");


                            }

                            double puckPosX = 0;
                            double puckPosY = 0;

                            foreach (var puck in tick.Pucks)
                            {

                                writetext.WriteLine("changepuck" + puck.Index.ToString() + frame.ToString() + "=bpy.data.objects[\"basepuck\"]");
                                writetext.WriteLine(String.Format("changepuck" + puck.Index.ToString() + frame.ToString() + ".location=({0},{1},{2})", puck.PosX, puck.PosZ, puck.PosY));
                                writetext.WriteLine("changepuck" + puck.Index.ToString() + frame.ToString() + ".keyframe_insert(data_path=\"location\", index=-1)");


                                writetext.WriteLine(String.Format("changepuck" + puck.Index.ToString() + frame.ToString() + ".rotation_euler=({0}, {1}, {2})", puck.RotX, puck.RotY, puck.RotZ));
                                writetext.WriteLine("changepuck" + puck.Index.ToString() + frame.ToString() + ".keyframe_insert(data_path=\"rotation_euler\", index=-1)");

                                puckPosX = puck.PosX;
                                puckPosY = puck.PosZ;
                            }

                            decimal posY = 0;

                            posY = Math.Clamp(posY, 0, 60);


                            frame += 1;
                        }
                    }
                    else
                    {
                        if (index % 4 == 0)
                        {
                            writetext.WriteLine("basepuck" + frame.ToString() + "=bpy.data.objects[\"basepuck\"]");
                            writetext.WriteLine(String.Format("basepuck" + frame.ToString() + ".location=({0},{1},{2})", 15, 30.5, 4));
                            writetext.WriteLine("basepuck" + frame.ToString() + ".keyframe_insert(data_path=\"location\", index=-1)");
                            writetext.WriteLine("scene.frame_set(" + frame.ToString() + ")");

                            frame += 1;
                        }
                    }

                    cameraOffset -= cameraTickStep;

                    index += 1;
                }

                foreach (var tick in replayClass.Ticks)
                {
                    if (index % 4 == 0)
                    {
                        writetext.WriteLine("scene.frame_set(" + frame.ToString() + ")");


                        foreach (var player in tick.Players)
                        {
                            var changeupper = "changeupper" + player.Index.ToString() + frame.ToString();

                            writetext.WriteLine(changeupper + "=bpy.data.objects[\"upper" + player.Index + "\"]");
                            writetext.WriteLine(String.Format(changeupper + ".location=({0},{1},{2})", (decimal)player.PosX, (decimal)player.PosZ, (decimal)player.PosY));
                            writetext.WriteLine(changeupper + ".keyframe_insert(data_path=\"location\", index=-1)");

                            var changelower = "changelower" + player.Index.ToString() + frame.ToString();

                            writetext.WriteLine(changelower + "=bpy.data.objects[\"lower" + player.Index + "\"]");
                            writetext.WriteLine(String.Format(changelower + ".location=({0},{1},{2})", (decimal)player.PosX, (decimal)player.PosZ, (decimal)player.PosY));
                            writetext.WriteLine(changelower + ".keyframe_insert(data_path=\"location\", index=-1)");


                            writetext.WriteLine(String.Format(changelower + ".rotation_euler=({0}, {1}, {2})", player.RotX, player.RotY, player.RotZ));
                            writetext.WriteLine(changelower + ".keyframe_insert(data_path=\"rotation_euler\", index=-1)");

                            writetext.WriteLine(String.Format(changeupper + ".rotation_euler=({0}, {1}, {2})", player.RotX - player.BodyLean, player.RotY - player.HeadTurn, player.RotZ));
                            writetext.WriteLine(changeupper + ".keyframe_insert(data_path=\"rotation_euler\", index=-1)");

                            writetext.WriteLine("changestick" + player.Index.ToString() + frame.ToString() + "=bpy.data.objects[\"stick" + player.Index + "\"]");
                            writetext.WriteLine(String.Format("changestick" + player.Index.ToString() + frame.ToString() + ".location=({0},{1},{2})", (decimal)player.StickPosX, (decimal)player.StickPosZ, (decimal)player.StickPosY));
                            writetext.WriteLine("changestick" + player.Index.ToString() + frame.ToString() + ".keyframe_insert(data_path=\"location\", index=-1)");

                            writetext.WriteLine(String.Format("changestick" + player.Index.ToString() + frame.ToString() + ".rotation_euler=({0}, {1}, {2})", player.StickRotX, player.StickRotY, player.StickRotZ));
                            writetext.WriteLine("changestick" + player.Index.ToString() + frame.ToString() + ".keyframe_insert(data_path=\"rotation_euler\", index=-1)");


                        }

                        double puckPosX = 0;
                        double puckPosY = 0;



                        foreach (var puck in tick.Pucks)
                        {

                            writetext.WriteLine("changepuck" + puck.Index.ToString() + frame.ToString() + "=bpy.data.objects[\"basepuck\"]");
                            writetext.WriteLine(String.Format("changepuck" + puck.Index.ToString() + frame.ToString() + ".location=({0},{1},{2})", puck.PosX, puck.PosZ, puck.PosY));
                            writetext.WriteLine("changepuck" + puck.Index.ToString() + frame.ToString() + ".keyframe_insert(data_path=\"location\", index=-1)");


                            writetext.WriteLine(String.Format("changepuck" + puck.Index.ToString() + frame.ToString() + ".rotation_euler=({0}, {1}, {2})", puck.RotX, puck.RotY, puck.RotZ));
                            writetext.WriteLine("changepuck" + puck.Index.ToString() + frame.ToString() + ".keyframe_insert(data_path=\"rotation_euler\", index=-1)");

                            puckPosX = puck.PosX;
                            puckPosY = puck.PosZ;
                        }


                        decimal posY = 0;
                       

                        posY = Math.Clamp(posY, 0, 60);


                        frame += 1;
                    }

                    cameraOffset -= cameraTickStep;

                    index += 1;
                }


                //writetext.WriteLine("bpy.ops.render.render(animation=True, write_still = True)");
                //writetext.WriteLine("bpy.ops.wm.quit_blender()");

            }
        }

        [HttpPost("ProcessHrp")]
        public async Task ProcessHrpAsync([FromForm] Guid gameId, [FromForm] string token, [FromForm] IFormFile replay)
        {
            Log.Information(LogHelper.GetInfoLog("Replay gameId: " + gameId));
            if (replay.Length > 0)
            {
                await _replayService.PushReplay(gameId, replay, token);
            }
        }

        [HttpPost("GetReplayViewer")]
        public async Task<IActionResult> GetReplayViewer(ReplayViewerRequest request)
        {
            var result = await _replayService.GetReplayViewer(request);

            return Ok(result);
        }

        [HttpPost("GetReplayGoals")]
        public async Task<IActionResult> GetReplayGoals(ReplayRequest request)
        {
            var result = await _replayService.GetReplayGoals(request);

            return Ok(result);
        }

        [HttpPost("GetReplayChatMessages")]
        public async Task<IActionResult> GetReplayChatMessages(ReplayRequest request)
        {
            var result = await _replayService.GetReplayChatMessages(request);

            return Ok(result);
        }
        [HttpPost("GetReplayHighlights")]
        public async Task<IActionResult> GetReplayHighlights(ReplayRequest request)
        {
            var result = await _replayService.GetReplayHighlights(request);

            return Ok(result);
        }

        [HttpPost("GetReplayStories")]
        public async Task<IActionResult> GetReplayStories()
        {
            var result = await _replayService.GetReplayStories();

            return Ok(result);
        }

        [HttpPost("GetStoryReplayViewer")]
        public async Task<IActionResult> GetStoryReplayViewer(StoryReplayViewerRequest request)
        {
            var result = await _replayService.GetStoryReplayViewer(request);

            return Ok(result);
        }

        [HttpPost("GetReportViewer")]
        public async Task<IActionResult> GetReportViewer(ReportViewerRequest request)
        {
            var result = await _replayService.GetReportViewer(request);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("LikeStory")]
        public async Task<IActionResult> LikeStory(StoryLikeRequest request)
        {
            var userId = UserHelper.GetUserId(User);
            await _replayService.LikeStory(request.Id, userId);

            return Ok();
        }
    }
}
