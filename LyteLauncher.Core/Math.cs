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

            var hEnd = h == 1 ? "" : "s";
            var mEnd = m == 1 ? "" : "s";
            var sEnd = s == 1 ? "" : "s";

            if (h > 0) return $"{h} Hour{hEnd} {m} Minutes{mEnd}";
            if (m > 0) return $"{m} Minute{mEnd}";
            return $"{s} Second{sEnd}";
        }
    }
}
