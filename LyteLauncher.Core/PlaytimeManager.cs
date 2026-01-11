using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LyteLauncher.Core
{
    internal class PlaytimeManager(Guid guid)
    {
        private Stopwatch stopwatch = new();

        public void Start() 
        {
            stopwatch.Start();
        }

        public double Stop()
        {
            stopwatch.Stop();
            var gametime = Games.GetTimeFromGUID(guid);

            return stopwatch.Elapsed.TotalSeconds + gametime;
        }
    }
}
