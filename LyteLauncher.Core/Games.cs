
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace LyteLauncher.Core
{
    internal class Games
    {
        public static double GetTimeFromGUID(Guid guid)
        {
            var game = GetFromGUID(guid);
            if (game != null)
            {
                return game.TotalPlayTime;
            }
            return 0;
        }

        public static GameListData? GetFromGUID(Guid guid)
        {
            var games = DataManager.GetGamesList();
            if (games != null)
            {
                var game = games.Find((p) => p.Id == guid);
                if (game != null)
                {
                    return game;
                }
            }
            return null;
        }

        public static GameListData? GetFromName(string name)
        {
            var games = DataManager.GetGamesList();
            if (games != null)
            {
                var game = games.Find((p) => p.Name == name);
                if (game != null)
                {
                    return game;
                }
            }
            return null;
        }

        public static bool UpdateTime(Guid id, double time)
        {
            var list = DataManager.GetGamesList();
            if (list is null) return false;
            var updated = false;

            foreach (var game in list)
            {
                if (updated) continue;
                if (game.Id == id)
                {
                    game.TotalPlayTime += time;
                    updated = true;
                }
            }

            if (updated) 
            {
                File.WriteAllText(DataManager.JsonGamesFile, JsonSerializer.Serialize(list));
            }
            return updated;
        }
    }
}
