using System;
using System.Collections.Generic;
using System.IO;
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

namespace FileName
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

        string dir;

        private void textBoxDir_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dir == textBoxDir.Text || !Directory.Exists(textBoxDir.Text))
                return;

            dir = textBoxDir.Text;

            updateFileList();
        }

        private void updateFileList()
        {
            var folder = new DirectoryInfo(dir);
            var files = folder.GetFiles();

            listView_SelectedFiles.Items.Clear();
            listView_UnSelectedFiles.Items.Clear();

            if (files.Length == 0)
            {
                listView_SelectedFiles.Items.Add("No file in this folder");
                return;
            }

            foreach (var file in files)
            {
                var n = file.Name;
                if (n.Contains('-'))
                {
                    listView_UnSelectedFiles.Items.Add(n);
                }
                else
                {
                    listView_SelectedFiles.Items.Add(n);
                }
            }
        }

        private void buttonAddDateTaken_Clicked(object sender, RoutedEventArgs e)
        {
            if (listView_SelectedFiles.Items.Count == 0)
            {
                MessageBox.Show("No file selected!");
                return;
            }

            buttonAddDateTaken.IsEnabled = false;
            changePictureNameByDateShot();
            buttonAddDateTaken.IsEnabled = true;
        }

        private void changePictureNameByDateShot()
        {
           foreach (string fn in listView_SelectedFiles.Items)
            {
                string on = System.IO.Path.Combine(dir, fn);

                try
                {
                    string nn;
                    // Open a Stream and decode a JPEG image
                    using (var fileStream = new FileStream(on, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        JpegBitmapDecoder decoder = new JpegBitmapDecoder(fileStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                        var metadata = decoder.Frames[0].Metadata as BitmapMetadata;
                        var sDate = metadata.DateTaken;
                        DateTime date = DateTime.Parse(sDate);
                        var d = date.ToString("MM.dd-");
                        nn = d + fn;
                        nn = System.IO.Path.Combine(dir, nn);
                    }

                    File.Move(on, nn);
                }
                catch
                {
                    continue;
                }
            }
            updateFileList();
        }
        private void mainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.PreviousSize.Height != 0)
            {
                var h = (e.NewSize.Height - e.PreviousSize.Height);
                listView_SelectedFiles.Height += h;
                listView_UnSelectedFiles.Height += h;
            }
        }
    }
}
