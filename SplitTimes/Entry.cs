using System;
using System.Text;
using System.Collections.Generic;
using Spectrum.API;
using Spectrum.API.Game;
using Spectrum.API.Game.Vehicle;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;

namespace SplitTimes
{
    public class Entry : IPlugin
    {
        public string FriendlyName => "Split times";
        public string Author => "Ciastex";
        public string Contact => "ciastexx@live.com";
        public APILevel CompatibleAPILevel => APILevel.RadioWave;

        private readonly List<TimeSpan> _previousCheckpointTimes = new List<TimeSpan>();

        public void Initialize(IManager manager)
        {
            LocalVehicle.CheckpointPassed += Vehicle_CheckpointPassed;
            Race.Started += Race_Started;
        }

        private void Race_Started(object sender, EventArgs e)
        {
            _previousCheckpointTimes.Clear();
            _previousCheckpointTimes.Add(TimeSpan.Zero);
        }

        private void Vehicle_CheckpointPassed(object sender, EventArgs e)
        {
            TimeSpan now = Race.Elapsed();

            if (now == TimeSpan.Zero)
                return;

            _previousCheckpointTimes.Add(now);

            LocalVehicle.Screen.SetTimeBarText($"{now.Minutes:D2}:{now.Seconds:D2}:{now.Milliseconds.ToString("D2").Substring(0, 2)}", "#0FA6D9", 1.25f);

            StringBuilder times = new StringBuilder();
            times.Append("<size=50></size>");
            times.Append("<size=57><color=#6be584ff>Regenerating</color></size>");
            times.AppendLine();

            for (int i = _previousCheckpointTimes.Count - 1; i > 0; i--){
                now = _previousCheckpointTimes[i];
                times.Append($"<size=25>{now.Minutes:D2}:{now.Seconds:D2}.{now.Milliseconds.ToString("D3").Substring(0, 3)}</size>");
                times.Append($"  ");

                now -= _previousCheckpointTimes[i - 1];
                times.Append($"<size=50>{now.Minutes:D2}:{now.Seconds:D2}.{now.Milliseconds.ToString("D3").Substring(0, 3)}</size>");
                times.AppendLine();
                times.Insert(9, Environment.NewLine); // 9 is the amount of characters in the first open size tag
            }

            LocalVehicle.HUD.Clear();
            LocalVehicle.HUD.SetHUDText(times.ToString(), 2.0f);
        }

        public void Shutdown()
        {
        }
    }
}
