using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LyteLauncher.Core
{
    public class RobloxHandler
    {
        public static string RobloxTimingsFile = $"{DataManager.LauncherPath()}/rblx_t";
        public static string RobloxPlayerExecutable = $"{DataManager.RobloxAppDir}/RobloxPlayerBeta.exe";

        public static string Tag = "RBLX";

        public RobloxHandler()
        {
            if (!File.Exists(RobloxTimingsFile))
            {
                File.WriteAllText(RobloxTimingsFile, "0");
            }
        }

        public static void UpdateTiming(double timing)
        {
            File.WriteAllText(RobloxTimingsFile, timing.ToString());
        }

        public static double GetTimings()
        {
            return double.Parse(File.ReadAllText(RobloxTimingsFile));
        }
    }
}
