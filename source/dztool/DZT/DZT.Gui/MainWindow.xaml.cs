using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IOPath = System.IO.Path;
using IOFile = System.IO.File;
using IODir = System.IO.Directory;
using System.Text.Json;

namespace DZT.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _prefsJsonPath;

        class AppPrefs
        {
            private readonly string _path;
            public AppPrefs(string path) => _path = path;

            public string DayzServerRootDirectoryPath { get; set; } = "";
            public string MpMissionName { get; set; } = "";

            public void Load()
            {
                var loaded = JsonSerializer.Deserialize<AppPrefs>(IOFile.ReadAllText(_path))
                     ?? throw new ApplicationException("");

                this.DayzServerRootDirectoryPath = loaded.DayzServerRootDirectoryPath;
                this.MpMissionName = loaded.MpMissionName;
            }

            public void Store()
            {
                IOFile.WriteAllText(_path, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
            }
        }

        private AppPrefs Prefs { get; set; }

        public MainWindow()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDir = IOPath.Combine(appDataDir, "DZT");
            _prefsJsonPath = IOPath.Combine(appDir, "prefs.json");
            if (!IODir.Exists(appDir))
            {
                IODir.CreateDirectory(appDir);
            }

            Prefs = new AppPrefs(_prefsJsonPath);
            if (!IOFile.Exists(_prefsJsonPath))
            {
                Prefs.Store();
            }
            else
            {
                Prefs.Load();
            }

            InitializeComponent();
            DayzServerRootDirTextBox.Text = Prefs.DayzServerRootDirectoryPath;
            MpMissionNameTextBox.Text = Prefs.MpMissionName;
        }

        private void SetDayzServerRootDirButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FolderPicker();
            if (picker.ShowDialog() is true)
            {
                DayzServerRootDirTextBox.Text = picker.ResultPath;
                Prefs.DayzServerRootDirectoryPath = picker.ResultPath;
                Prefs.Store();
            }
        }

        private void SetMpMissionFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FolderPicker { InputPath = DayzServerRootDirTextBox.Text };
            if (picker.ShowDialog() is true)
            {
                MpMissionNameTextBox.Text = picker.ResultPath;
                Prefs.MpMissionName = picker.ResultPath;
                Prefs.Store();
            }
        }
    }
}
