using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using MFontFamily = System.Windows.Media.FontFamily;
using IOFile = System.IO.File;

namespace HED.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _editor.ShowLineNumbers = true;
            if (AssetHelpers.TestFont("Fira Code"))
            {
                _editor.FontFamily = new MFontFamily("Fira Code");
            }

            _editor.Text = IOFile.ReadAllText(@"C:\Program Files (x86)\Steam\steamapps\common\DayZServer\config\SearchForLoot\SearchForLoot.json");
            _editor.Focus();
            EventManager.RegisterClassHandler(typeof(MainWindow), Keyboard.KeyDownEvent, new RoutedEventHandler(_routedEventHandler));
        }

        private void _routedEventHandler(object sender, RoutedEventArgs e)
        {
            UpdateStatusBar();
        }

        private void UpdateStatusBar()
        {
            var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);
            var currentLocation = _editor.Document.GetLocation(_editor.CaretOffset);
            statusBar_CursorPosition.Text = $"C:{_editor.CaretOffset} | L:{currentLine} | L:{currentLocation}";
        }

        private void _editor_KeyDown(object sender, KeyEventArgs e)
        {
            UpdateStatusBar();
        }

        private void _editor_StylusMove(object sender, StylusEventArgs e)
        {
            UpdateStatusBar();
        }

        private void _editor_TextChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        private void _editor_DocumentChanged(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }
    }
}
