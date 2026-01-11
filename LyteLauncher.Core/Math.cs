using System;
using System.Collections.Generic;
using System.Text;

namespace LyteLauncher.Core
{
    internal class Math
    {
        public static string ParseTime(double time)
        {
            int h = (int)(time / 3600);
            int m = (int)((time % 3600) / 60);
            int s = (int)(time % 60);

            if (h > 0) return $"{h} Hours {m} Minutes";
            if (m > 0) return $"{m} Minutes";
            return $"{s} Seconds";
        }
    }
}
