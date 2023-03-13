using Microsoft.Win32;
using PEt.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PEt.View
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private Service service;
        private BitmapImage uploadedImageBitmap;
        private string uploadedImageFilename;
        private byte[] imageData;

        public EditWindow(Service service)
        {
            InitializeComponent();

            this.service = service;

            if (service != null)
            {
                LoadCbCategories();

                tbName.Text = service.ServiceName.ToString();
                tbPrice.Text = service.ServicePrice.ToString();
                tbDiscount.Text = service.ServiceDiscount.ToString();
                cbCategory.SelectedIndex = service.ServiceCategoryId-1;
                

                btnLoadNewService.Content = "Редактировать";

                byte[] imageData = service.ServiceImage; // массив байт
                if (imageData != null && imageData.Length > 0)
                {
                    BitmapImage image = new BitmapImage();
                    using (MemoryStream memoryStream = new MemoryStream(imageData))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = memoryStream;
                        image.EndInit();
                    }
                    imageExample.Source = image;
                }
                this.imageData = imageData;
            }
        }

        private void LoadCbCategories()
        {
            using (DataModel context = new DataModel())
            {
                var categories = context.ServiceCategories.
                Select(x => x.ServiceCategoryName).
                ToList();

                cbCategory.ItemsSource = categories;
            }
        }

        private void LoadService()
        {

            if (service == null)
            {
                using (DataModel context = new DataModel())
                {
                    Service newService = new Service()
                    {
                        ServiceName = tbName.Text,
                        ServicePrice = Convert.ToInt32(tbPrice.Text),
                        ServiceCategoryId = cbCategory.SelectedIndex+1,
                        ServiceImage = imageData,
                        ServiceDiscount = (int?)Convert.ToInt64(tbDiscount.Text)
                    };

                    context.Services.Add(newService);

                    context.SaveChanges();
                }
            }
            else
            {
                using (DataModel context = new DataModel())
                {
                    Service servicetot = context.Services.Find(Convert.ToInt32(service.ServiceId));

                    servicetot.ServiceName = tbName.Text;
                    servicetot.ServicePrice = Convert.ToInt32(tbPrice.Text.ToString());
                    servicetot.ServiceCategoryId = cbCategory.SelectedIndex+1;
                    servicetot.ServiceImage = imageData;
                    try
                    {
                        servicetot.ServiceDiscount = (int?)Convert.ToInt64(tbDiscount.Text.ToString());
                    }
                    catch (Exception)
                    {
                        servicetot.ServiceDiscount = null;

                    }

                    context.SaveChanges();                    
                }
            }
            this.Close();
        }

        private void LoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg; *.jpeg; *.png; *.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                uploadedImageBitmap = new BitmapImage(new Uri(openFileDialog.FileName));

                uploadedImageFilename = openFileDialog.SafeFileName;
                imageData = File.ReadAllBytes(openFileDialog.FileName);

                imageExample.Source = uploadedImageBitmap;
            }

            uploadedImageName.Text += uploadedImageFilename.Trim();
            uploadedImageName.Visibility = Visibility.Visible;
        }

        private void btnLoadImage_Click(object sender, RoutedEventArgs e)
        {
            LoadImage();
        }

        private void btnLoadNewService_Click(object sender, RoutedEventArgs e)
        {
            LoadService();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
