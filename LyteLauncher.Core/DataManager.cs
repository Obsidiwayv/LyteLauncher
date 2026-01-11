using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace LyteLauncher.Core
{
    public class GameListData
    {
        public string? IconSrc { get; set; }
        public required string ExecutablePath { get; set; }
        public required string Name { get; set; }
        public double TotalPlayTime { get; set; }
        public Guid Id { get; set; }
    }

    internal class DataManager
    {
        public static string JsonGamesFile = $"{LauncherPath()}/games.json";
        public static string IndexFile = $"{LauncherPath()}/index";

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
