using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Reflection;
using Spectrum.API;
using Spectrum.API.Game;
using Spectrum.API.Game.Vehicle;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using Spectrum.API.Configuration;
using Spectrum.API.FileSystem;
using Spectrum.API.Game.EventArgs.Vehicle;
using UnityEngine;

namespace SplitTimes
{
    public class Entry : IPlugin
    {
        public string FriendlyName => "Split times";
        public string Author => "Jonathan Vollebregt";
        public string Contact => "jnvsor@gmail.com";
        public APILevel CompatibleAPILevel => APILevel.MicroWave;

        private readonly List<TimeSpan> _previousCheckpointTimes = new List<TimeSpan>();
        private PluginData PluginData { get; set; }
        private Settings Settings { get; set; }

        public void Initialize(IManager manager)
        {
            LocalVehicle.CheckpointPassed += LocalVehicle_CheckpointPassed;
            LocalVehicle.Finished += LocalVehicle_Finished;
            Race.Started += Race_Started;

            PluginData = new PluginData(typeof(Entry));
            Settings = new Settings(typeof(Entry));
            ValidateSettings();

            manager.Hotkeys.Bind(Settings["ShowTimesHotkey"], SplitTimes_ShowPressed, false);
        }

        private void Race_Started(object sender, EventArgs e)
        {
            _previousCheckpointTimes.Clear();
            _previousCheckpointTimes.Add(TimeSpan.Zero);
        }

        private void LocalVehicle_Finished (object sender, FinishedEventArgs e)
        {
            if (e.Type != RaceEndType.Finished)
                return;

            var finished = TimeSpan.FromMilliseconds(e.FinalTime);

            if (Settings["SaveTimes"] == "true")
            {
                try
                {
                    var trackname = Regex.Replace(G.Sys.GameManager_.Level_.Name_, "[<>:\"/\\\\|?*_ ]+", "_");
                    var filename = trackname + Path.DirectorySeparatorChar +
                        $"{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}_" +
                        $"{finished.Minutes:D2}-{finished.Seconds:D2}-{finished.Milliseconds.ToString("D3").Substring(0, 3)}.txt";

                    PluginData.CreateDirectory(trackname);
                    using (var sw = new StreamWriter(PluginData.CreateFile(filename)))
                    {
                        var now = finished - TimeSpan.FromMilliseconds(
                            Math.Round(_previousCheckpointTimes[_previousCheckpointTimes.Count - 1].TotalMilliseconds / 10f) * 10f
                        );
                        sw.Write($"{finished.Minutes:D2}:{finished.Seconds:D2}.{finished.Milliseconds.ToString("D3").Substring(0, 3)}\t");
                        sw.Write($"{now.Minutes:D2}:{now.Seconds:D2}.{now.Milliseconds.ToString("D3").Substring(0, 3)}");
                        sw.WriteLine();

                        for (int i = _previousCheckpointTimes.Count - 1; i > 0; i--)
                        {
                            now = _previousCheckpointTimes[i];
                            sw.Write($"{now.Minutes:D2}:{now.Seconds:D2}.{Math.Round(now.Milliseconds / 10f).ToString("D2").Substring(0, 2)}\t");

                            now -= _previousCheckpointTimes[i - 1];
                            sw.Write($"{now.Minutes:D2}:{now.Seconds:D2}.{Math.Round(now.Milliseconds / 10f).ToString("D2").Substring(0, 2)}");

                            sw.WriteLine();
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"SplitTimes: Couldn't save times. Exception below:\n{ex}");
                }
            }

            _previousCheckpointTimes.Add(finished);
        }

        private void LocalVehicle_CheckpointPassed(object sender, EventArgs e)
        {
            var now = Race.Elapsed();
            _previousCheckpointTimes.Add(now);

            LocalVehicle.Screen.SetTimeBarText($"{now.Minutes:D2}:{now.Seconds:D2}:{now.Milliseconds.ToString("D2").Substring(0, 2)}", "#0FA6D9", 1.25f);

            var times = GetTimeStrings();

            var output = new StringBuilder();
            output.Append("<size=57><color=#6be584ff>Regenerating</color></size>");
            output.AppendLine();
            output.Append(String.Join(Environment.NewLine, times.ToArray()));
            for (int i = 0; i < times.Count; i++)
                output.Insert(0, Environment.NewLine);

            LocalVehicle.HUD.Clear();
            LocalVehicle.HUD.SetHUDText(output.ToString(), 2.0f);
        }

        private void SplitTimes_ShowPressed()
        {
            if (Race.Elapsed() >= _previousCheckpointTimes.Last())
                ShowTimes(2.5f);
        }

        private void ShowTimes(float delay)
        {
            var times = GetTimeStrings();
            times.Insert(0, GetTimeString(Race.Elapsed(), _previousCheckpointTimes.Last()));

            var output = new StringBuilder();
            output.Append("<size=57><color=#00000000>Regenerating</color></size>"); // Dummy to keep it at the same height
            output.AppendLine();
            output.Append(String.Join(Environment.NewLine, times.ToArray()));
            for (int i = 0; i < times.Count; i++)
                output.Insert(0, Environment.NewLine);

            LocalVehicle.HUD.Clear();
            LocalVehicle.HUD.SetHUDText(output.ToString(), delay);

        }

        private List<string> GetTimeStrings()
        {
            var output = new List<string>();

            for (int i = _previousCheckpointTimes.Count - 1; i > 0; i--)
                output.Add(GetTimeString(_previousCheckpointTimes[i], _previousCheckpointTimes[i - 1]));

            return output;
        }

        private string GetTimeString(TimeSpan now, TimeSpan last)
        {
            var diff = now - last;

            return $"<size=25>{now.Minutes:D2}:{now.Seconds:D2}.{now.Milliseconds.ToString("D3").Substring(0, 3)}</size>" +
                "  " +
                $"{diff.Minutes:D2}:{diff.Seconds:D2}.{diff.Milliseconds.ToString("D3").Substring(0, 3)}";
        }

        public void Shutdown()
        {
        }

        private void ValidateSettings()
        {
            if (Settings["ShowTimesHotkey"] == string.Empty)
                Settings["ShowTimesHotkey"] = "LeftControl+X";

            if (Settings["SaveTimes"] == string.Empty)
                Settings["SaveTimes"] = "false";

            Settings.Save();
        }
    }
}
