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
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
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
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
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
}
