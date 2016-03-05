using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Spectrum.API.Game;
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

        private DateTime _startedTime;
        private DateTime _currentCheckpointTime;
        private DateTime _previousCheckpointTime = default(DateTime);

        public void Initialize(IManager manager)
        {
            Vehicle.CheckpointPassed += Vehicle_CheckpointPassed;
            Race.Started += Race_Started;
        }

        private void Race_Started(object sender, EventArgs e)
        {
            _startedTime = DateTime.Now;
        }

        private void Vehicle_CheckpointPassed(object sender, EventArgs e)
        {
            _currentCheckpointTime = DateTime.Now;

            TimeSpan timeBetweenCheckpoints;
            if (_previousCheckpointTime == DateTime.MinValue)
            {
                timeBetweenCheckpoints = DateTime.Now - _startedTime;
            }
            else
            {
                timeBetweenCheckpoints = _currentCheckpointTime - _previousCheckpointTime;
            }
            _previousCheckpointTime = _currentCheckpointTime;

            Vehicle.SetTimeBarText($"{timeBetweenCheckpoints.Minutes.ToString("D2")}:{timeBetweenCheckpoints.Seconds.ToString("D2")}.{timeBetweenCheckpoints.Milliseconds.ToString("D4")}", "#0FA6D9", 2.0f);
        }

        public void Shutdown()
        {
            
        }
    }
}
