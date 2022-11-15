using SAK;
using System;
using System.IO;
using System.Text.Json;

namespace DZT.Gui
{
    internal class AppPrefs
    {
        private readonly string _path = "";
        internal AppPrefs(string path) => _path = path;
        public AppPrefs() { }

        public int Version { get; set; } = 1;
        public string DayzServerRootDirectoryPath { get; set; } = "";
        public string MpMissionName { get; set; } = "";

        public void Load()
        {
            var loaded = JsonSerializer.Deserialize<AppPrefs>(File.ReadAllText(_path))
                 ?? throw new ApplicationException($"Error reading '{_path}'");

            this.DayzServerRootDirectoryPath = loaded.DayzServerRootDirectoryPath;
            this.MpMissionName = loaded.MpMissionName;
        }

        public void Store()
        {
            File.WriteAllText(_path, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }

        internal void EnsureInited()
        {
            var appDir = Path.GetDirectoryName(_path).OrFail();
            if (!Directory.Exists(appDir))
            {
                Directory.CreateDirectory(appDir);
            }

            if (!File.Exists(_path))
            {
                this.Store();
            }
            else
            {
                this.Load();
            }
        }
    }
}
