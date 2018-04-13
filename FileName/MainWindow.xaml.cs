using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
                string on = Path.Combine(dir, fn);

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
                        nn = Path.Combine(dir, nn);
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

        private void changePictureNameByDateShot2()
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            foreach (string fn in listView_SelectedFiles.Items)
            {
                string on = Path.Combine(dir, fn);
                string t,
                       exifDTOrig = "",
                       dateTime = "";
                try
                {
                    using (var image = System.Drawing.Image.FromFile(on))
                    {
                        foreach (var pi in image.PropertyItems)
                        {
                            if (pi.Id == 0x0132)
                                dateTime = encoding.GetString(pi.Value);
                            if (pi.Id == 0x9003)
                            {
                                exifDTOrig = encoding.GetString(pi.Value);
                                break;
                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
                if (exifDTOrig != "")
                    t = exifDTOrig;
                else
                    t = dateTime;
                t = t.Substring(0, 19);
                DateTime date = DateTime.ParseExact(t, "yyyy:MM:dd hh:mm:ss",
                    CultureInfo.InvariantCulture);
                
                var d = date.ToString("MM.dd-");
                string nn;
                nn = d + fn;
                File.Move(on, Path.Combine(dir, nn));
            }
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
