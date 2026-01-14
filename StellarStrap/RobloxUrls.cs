using System;
using System.Collections.Generic;
using System.Text;

namespace StellarStrap
{
    internal class RobloxUrls
    {
        public static string Binary { get; } = "https://clientsettingscdn.roblox.com/v2/client-version/WindowsPlayer";

        public static string GetVersionManifest(string version)
        {
            return $"https://setup.rbxcdn.com/channel/common/{version}-rbxPkgManifest.txt";
        }
    }
}
