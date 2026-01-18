using RivenSDK.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace LyteLauncher.Core
{
    public enum GameType
    {
        Common,
        Roblox
    }

    public class GameListData
    {
        public string? IconSrc { get; set; }
        public required string ExecutablePath { get; set; }
        public required string Name { get; set; }
        public double TotalPlayTime { get; set; }
        public Guid Id { get; set; }
    }

    public class UserSettingJSON
    {
        public string CurrentBootstrapPreset { get; set; }
    }

    public class LoadedVirtualGameCard : GameListData
    {
        public GameType Type = GameType.Common;

        public static LoadedVirtualGameCard FromList(GameListData list)
        {
            return new LoadedVirtualGameCard()
            {
                ExecutablePath = list.ExecutablePath,
                IconSrc = list.IconSrc,
                Id = list.Id,
                Name = list.Name,
                TotalPlayTime = list.TotalPlayTime
            };
        }
    }

    public delegate UserSettingJSON UserSaveCatalyst(UserSettingJSON json);

    internal class DataManager
    {
        public static string JsonGamesFile = $"{LauncherPath()}/games.json";
        public static string IndexFile = $"{LauncherPath()}/index";
        public static string AppsDir = $"{LauncherPath()}/Apps";
        public static string RobloxAppDir = $"{AppsDir}/Stella";
        public static string RobloxZipFileCacheDir = $"{LauncherPath()}/Download/RobloxClient";

        public static void InitDirectories()
        {
            string[] dirs = [RobloxAppDir, RobloxZipFileCacheDir];
            foreach (string dir in dirs)
            {
                RivenFS.CheckDirectory(dir);
            }
        }

        public static void WriteGame(GameListData game)
        {
            List<GameListData> list;

            if (File.Exists(JsonGamesFile))
            {
                list = GetGamesList()!;
                list.Add(game);
                File.WriteAllText(JsonGamesFile, JsonSerializer.Serialize(list));
                return;
            }
            else
            {
                list =
                [
                    game
                ];
                File.WriteAllText(JsonGamesFile, JsonSerializer.Serialize(list));
            }
        }

        public static string LauncherPath()
        {
            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}/Wayvlyte/Launcher";
            RivenFS.CheckDirectory(path);
            return path;
        }

        public static List<GameListData>? GetGamesList()
        {
            if (!File.Exists(JsonGamesFile))
            {
                return [];
            }
            return JsonSerializer.Deserialize<List<GameListData>>(File.ReadAllText(JsonGamesFile));
        }
    }

    public class UserSettings
    {
        public static string UserSettingPath = $"{DataManager.LauncherPath()}/User";
        public static string UserSettingFile = $"{UserSettingPath}/Current.json";

        public static void Save(UserSaveCatalyst func)
        {
            var readJson = Get();
            if (readJson == null)
            {
                MessageBox.Show("Unable to get user settings", "SETTINGS Reader");
                return;
            }
            var json = func.Invoke(readJson);
            File.WriteAllText(UserSettingFile, JsonSerializer.Serialize(json));
        }

        public static UserSettingJSON? Get()
        {
            return JsonSerializer.Deserialize<UserSettingJSON>(File.ReadAllText(UserSettingFile));
        }

        public static void InitFile()
        {
            RivenFS.CheckDirectory(UserSettingPath);
            if (!File.Exists(UserSettingFile)) File.WriteAllText(UserSettingFile, "{}");
        }
    }
}
