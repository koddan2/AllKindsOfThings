using SAK;
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
using System.IO;
using DZT.Lib;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DZT.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AppPrefs Prefs { get; set; }

        public MainWindow()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDir = System.IO.Path.Combine(appDataDir, "DZT");
            var prefsJsonPath = System.IO.Path.Combine(appDir, "prefs.json");
            Prefs = new AppPrefs(prefsJsonPath);
            Prefs.EnsureInited();

            InitializeComponent();
            PopulateFromPrefs();
        }

        private void PopulateFromPrefs()
        {
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
                var result = new DirectoryInfo(picker.ResultPath).OrFail().Name;
                MpMissionNameTextBox.Text = result;
                Prefs.MpMissionName = result;
                Prefs.Store();
            }
        }

        private void OperationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OperationComboBox.SelectedItem is not null)
            {
                GoButton.IsEnabled = true;
            }
        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selItem = (ComboBoxItem)OperationComboBox.SelectedItem;
            if (selItem == AdjustTypes)
            {
                ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                {
                    //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, WpfLoggerProvider>());
                    builder.Services.AddSingleton<ILoggerProvider>(
                        (_) => new WpfLoggerProvider(this.InfoTextBox)
                    );
                });

                ILogger<AdjustTypesXml> logger = loggerFactory.CreateLogger<AdjustTypesXml>();
                AdjustTypesXml impl =
                    new(
                        logger,
                        new AdjustTypesXmlConfiguration(),
                        Prefs.DayzServerRootDirectoryPath,
                        Prefs.MpMissionName
                    );
                impl.Process();
            }
        }
    }
}
