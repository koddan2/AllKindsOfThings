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

namespace DZT.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetDayzServerRootDirButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FolderPicker();
            if (picker.ShowDialog() is true)
            {
                DayzServerRootDirTextBox.Text = picker.ResultPath;
            }
        }

        private void SetMpMissionFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FolderPicker { InputPath=DayzServerRootDirTextBox.Text};
            if (picker.ShowDialog() is true)
            {
                MpMissionNameTextBox.Text = picker.ResultPath;
            }
        }
    }
}
