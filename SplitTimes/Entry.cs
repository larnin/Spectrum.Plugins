using System;
using System.Text;
using System.Collections.Generic;
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
        public int CompatibleAPILevel => 1;

        private List<TimeSpan> PreviousCheckpointTimes = new List<TimeSpan>();

        public void Initialize(IManager manager)
        {
            Vehicle.CheckpointPassed += Vehicle_CheckpointPassed;
            Race.Started += Race_Started;
        }

        private void Race_Started(object sender, EventArgs e)
        {
            PreviousCheckpointTimes.Clear();
            PreviousCheckpointTimes.Add(TimeSpan.Zero);
        }

        private void Vehicle_CheckpointPassed(object sender, EventArgs e)
        {
            TimeSpan now = Race.Elapsed();

            if (now == TimeSpan.Zero)
                return;

            PreviousCheckpointTimes.Add(now);

            Vehicle.Screen.SetTimeBarText($"{now.Minutes:D2}:{now.Seconds:D2}:{now.Milliseconds.ToString("D2").Substring(0, 2)}", "#0FA6D9", 1.25f);

            StringBuilder times = new StringBuilder();
            times.Append("<size=50></size>");
            times.Append("<size=55><color=#6be584ff>Regenerating</color></size>");
            times.AppendLine();

            for (int i = PreviousCheckpointTimes.Count - 1; i > 0; i--){
                now = PreviousCheckpointTimes[i];
                times.Append($"<size=25>{now.Minutes:D2}:{now.Seconds:D2}.{now.Milliseconds.ToString("D3").Substring(0, 3)}</size>");
                times.Append($"  ");

                now -= PreviousCheckpointTimes[i - 1];
                times.Append($"<size=50>{now.Minutes:D2}:{now.Seconds:D2}.{now.Milliseconds.ToString("D3").Substring(0, 3)}</size>");
                times.AppendLine();
                times.Insert(9, Environment.NewLine); // 9 is the amount of characters in the first open size tag
            }

            Vehicle.HUD.Clear();
            Vehicle.HUD.SetHUDText(times.ToString(), 2.0f);
        }

        public void Shutdown()
        {
        }
    }
}
