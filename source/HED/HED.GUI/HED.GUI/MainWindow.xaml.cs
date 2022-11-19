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
using System.Text.Json;
using System.Text.Json.Serialization;

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
        }

        private void UpdateStatusBar()
        {
            var currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);
            var currentLocation = _editor.Document.GetLocation(_editor.CaretOffset);
            {
                var ta = _editor.TextArea;
                var opts = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.Preserve
                };
                var info = new
                {
                    A = ta.Cursor
                };
                _editor.Text = JsonSerializer.Serialize(ta, opts);
            }
            statusBar_CursorPosition.Text = $"::{_editor.CaretOffset} | ::{currentLine} | ::{currentLocation}";
        }

        private void _editor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            UpdateStatusBar();
        }

        private void _editor_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            UpdateStatusBar();
        }

        private void _editor_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateStatusBar();
        }
    }
}
