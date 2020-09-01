using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;

namespace AddInfoAndImage
{
    /// <summary>
    /// Interaction logic for AddInfo.xaml
    /// </summary>
    public partial class AddInfo : Window
    {
        private readonly ObservableCollection<byte[]> _imageData = new ObservableCollection<byte[]>();
        private bool _dialogState = false;
        private  string ImagesFolderPath;
        private static Dictionary<string, Bitmap> bitMap = new Dictionary<string, Bitmap>();
        public AddInfo()
        {
            ImagesFolderPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "..", "FeedManager.ImagesFolder");
            InitializeComponent();
        }

        private void AddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*",
                RestoreDirectory = true,
                Multiselect = true
            };

            if (dlg.ShowDialog() is true)
            {
                foreach (var file in dlg.FileNames)
                {
                    _imageData.Add(File.ReadAllBytes(file));
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _imageData.CollectionChanged += ImageData_CollectionChanged;
        }


        private void ImageData_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            images.Items.Clear();
            foreach (var item in sender as ObservableCollection<byte[]>)
            {
                ListViewItem listitem = new ListViewItem()
                {
                    Height = 130,
                    Content = new Image()
                    {
                        Source = ImageActions.ImageFromByteArray(item),
                        Stretch = Stretch.Uniform,
                    }
                };
                images.Items.Add(listitem);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = _dialogState;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(matchIdBox.Text) || string.IsNullOrWhiteSpace(categoryBox.Text))
            {
                matchIdBox.BorderBrush = Brushes.Red;
                categoryBox.BorderBrush = Brushes.Red;
                MessageBox.Show("Enter MatchId Or Select Category");
                return;
            }

            try
            {
                DetectByRiskManager lostItem = new DetectByRiskManager
                {
                    MatchId = Convert.ToInt32(matchIdBox.Text),
                    Category = Convert.ToInt32(categoryBox.Text),
                    Description = descriptionBox.Text,
                    RiskManagerName = "Varsik"

                };
                if (lostItem != null)
                {
                    List<ImageModel> data = new List<ImageModel>();
                    for (int i = 0; i < _imageData?.Count; i++)
                    {
                        data.Add(new ImageModel { Name = $"image{i}.png", Data = _imageData[i] });
                    }
                    foreach (var item in data)
                    {
                        using (MemoryStream ms = new MemoryStream(item.Data))
                        {
                            if (ms.Length > 0)
                            {
                                using (Bitmap bm = new Bitmap(ms))
                                {

                                    var newFolder = ImagesFolderPath + "MatchId=" + lostItem.MatchId.ToString() + "RiskManagerName=" + lostItem.RiskManagerName;
                                    if (!File.Exists(newFolder))
                                    {
                                        Directory.CreateDirectory(newFolder);
                                        newFolder = newFolder + @"\";
                                    }

                                    newFolder = newFolder + $"image{Guid.NewGuid()}.png";
                                    bm.Save(newFolder);

                                    if (string.IsNullOrEmpty(lostItem.ImagesPath))
                                    {
                                        lostItem.ImagesPath += newFolder;
                                    }
                                    else
                                    {
                                        lostItem.ImagesPath = lostItem.ImagesPath + ',' + newFolder;
                                    }
                                }
                            }
                        }
                    }

                    DialogResult = true;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something Went Wrong");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            _imageData.Clear();
        }

        private void Images_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                int selected = images.SelectedIndex;
                if (selected != -1)
                {
                    _imageData.RemoveAt(selected);
                }
            }
        }
    }
}
