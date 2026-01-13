using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Shapes;

namespace LyteLauncher.Core
{
    public class LoggerSector(string sectorName, string fileName) 
    {
        public void Write(string content)
        {
            var file = File.ReadAllText(fileName);
            file += $"[{DateTime.UtcNow:hh:mm:ss}] -> {sectorName} -> {content}\n";

            File.WriteAllText(fileName, file);
        }
    }

    public class Logger
    {
        private static string Folder { get; } = $"{DataManager.LauncherPath()}/Logs";

        private string FileName { get; } 
            = $"{Folder}/{DateTime.Now:ddhmmyyyy}.log";

        public Logger()
        {
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);

            File.Create(FileName);
        }

        public LoggerSector UseSector(string sectorName)
        {
            return new LoggerSector(sectorName, FileName);
        }
    }
}
