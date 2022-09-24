
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ChaosGalaxySpriteSheetLazyMaker
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        string OpenDialog(string title)
        {

            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Title = title;
            dialog.FileName = "Image"; // Default file name
            dialog.DefaultExt = ".png"; // Default file extension
            dialog.Filter = "PNG Files (.png)|*.png"; // Filter files by extension

            // Show open file dialog box

            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                if (dialog.FileName.Trim().Length == 0)
                {
                    return null;
                }
                // Open document
                return dialog.FileName;
            }

            return null;
        }
        string SaveDialog(string loadedImagePath)
        {
            string[] inifilesplit = loadedImagePath.Split('/');
            string inifile = inifilesplit[inifilesplit.Length - 1];
            int idx = inifile.LastIndexOf(".");
            if (idx != -1)
            {
                inifile = inifile.Substring(0, idx);
            }
            if (!inifile.EndsWith(" SpriteSheet"))
            {
                inifile += " SpriteSheet";
            }
            if (!inifile.EndsWith(".png"))
            {
                inifile += ".png";
            }

            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Title = "Save Spritesheet PNG";
            dialog.FileName = inifile; // Default file name
            dialog.Filter = "PNG Files (.png)|*.png"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                string resultpath = dialog.FileName;
                if (resultpath.Trim().Length == 0)
                {
                    return null;
                }
                else if (!resultpath.EndsWith(".png"))
                {
                    resultpath += ".png";
                }
                // Open document
                return resultpath;
            }

            return null;
        }


        void OnClickSprite(object sender, RoutedEventArgs e)
        {
            string filepath = OpenDialog("Select a unit sprite");
            if (filepath == null)
            {
                return;
            }
            string out_filepath = SaveDialog(filepath);
            if(out_filepath == null)
            {
                return;
            }
            SpriteSheetCG.CreateSpriteSheet(filepath, out_filepath);
        }
        void OnClickDismiss(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.D)
            {
                OnClickSprite(null, null);
            }

            else if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
