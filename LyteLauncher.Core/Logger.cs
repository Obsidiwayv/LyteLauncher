using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LyteLauncher.Core
{
    public class Logger
    {
        private static string Folder { get; } = $"{DataManager.LauncherPath()}/Logs";

        private string FileName { get; } 
            = $"{Folder}/{DateTime.Now:ddhmmyyyy}.log";

        private bool IsReady { get; set; } = false;

        private List<string> Queue { get; set; } = [];

        public Logger()
        {
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);

            File.Create(FileName);
            IsReady = true;
        }

        public void Write(string content)
        {
            if (!IsReady)
            {
                Queue.Add(content);
                return;
            }
            var file = File.ReadAllText(FileName);
            if (Queue.Count != 0)
            {
                foreach (string line in Queue)
                {
                    file += $"{line}\n";
                }
            } else
            {
                file += $"{content}\n";
            }
            File.WriteAllText(FileName, file);
        }
    }
}
