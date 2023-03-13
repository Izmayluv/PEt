using PEt.Entity;
using PEt.View;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Windows;
using System.Windows.Controls;


namespace PEt
{
    /// <summary>
    /// Логика взаимодействия для CatalogWindow.xaml
    /// </summary>
    public partial class CatalogWindow : Window
    {
        User customer;
        List<Service> selectedServices = new List<Service>();

        public CatalogWindow(User customer)
        {
            InitializeComponent();

            this.customer = customer;

            if(customer != null )
            {
                MessageBox.Show($"Вы вошли как {customer.UserRole1.UserRoleName}");
            }
        }

        private const int categoryTherapy = 1;
        private const int categoryGrooming = 2;
        private const int categorySurgery = 3;

        private const int discountFrom10 = 10;
        private const int discountFrom15 = 15;
        private const int discountFrom50 = 50;



        private void LoadListView()
        {
            using (DataModel context = new DataModel())
            {

                var services = context.Services.
                    ToList();

                int countBeforeSorting = services.Count;

                if (tbSearch.Text != "")
                {
                    services = services.
                        Where(x=>x.ServiceName.ToLower().
                        Contains(tbSearch.Text.ToLower())).
                        ToList();
                }

                switch (cbCategory.SelectedIndex)
                {
                    default:
                        break;
                    case 1:
                        services = services.
                            Where(x=>x.ServiceCategoryId == categoryTherapy).
                            ToList();
                        break;
                    case 2:
                        services = services.
                            Where(x => x.ServiceCategoryId == categoryGrooming).
                            ToList();
                        break;
                    case 3:
                        services = services.
                            Where(x => x.ServiceCategoryId == categorySurgery).
                            ToList();
                        break;
                }

                switch (cbPrice.SelectedIndex)
                {
                    default:
                        break;
                    case 1:
                        services = services.
                            OrderBy(x=>x.ServicePrice).
                            ToList();
                        break;
                    case 2:
                        services = services.
                            OrderByDescending(x => x.ServicePrice).
                            ToList();
                        break;
                }

                switch (cbDiscount.SelectedIndex)
                {
                    default:
                        break;
                    case 1:
                        services = services.
                            Where(x => x.ServiceDiscount >= discountFrom10).
                            ToList();
                        break;
                    case 2:
                        services = services.
                            Where(x => x.ServiceDiscount >= discountFrom15).
                            ToList();
                        break;
                    case 3:
                        services = services.
                            Where(x => x.ServiceDiscount >= discountFrom50).
                            ToList();
                        break;
                }
                
                listServices.ItemsSource = services;

                int countAfterSorting = services.Count;

                tbCount.Text = $"Выведено строк:\n {countAfterSorting} из {countBeforeSorting}";
            }
        }

        private void LoadCbCategories()
        {
            using (DataModel context = new DataModel()) // заполнение cbCategory
            {
                var categories = context.ServiceCategories.
                    Select(x => x.ServiceCategoryName).
                    ToList();

                categories.Insert(0, "Все категории");
                cbCategory.Items.Refresh();

                cbCategory.ItemsSource = categories;
                cbCategory.SelectedItem = cbCategory.Items[0];
            }
        }

        private void LoadCbPrice()
        {
            cbPrice.Items.Add("Без сортировки");
            cbPrice.Items.Add("По возрастанию");
            cbPrice.Items.Add("По убыванию");

            cbPrice.SelectedItem = cbPrice.Items[0];
        }

        private void LoadCbDiscount()
        {
            cbDiscount.Items.Add("Любая скидка");
            cbDiscount.Items.Add("от 10%");
            cbDiscount.Items.Add("от 15%");
            cbDiscount.Items.Add("от 50%");

            cbDiscount.SelectedItem = cbDiscount.Items[0];

        }

        private void DeleteItem()
        {
            var selectedItem = listServices.SelectedItem;

            using (DataModel context = new DataModel())
            {
                if (selectedItem != null)
                {
                    int selectedItemId = ((Service)selectedItem).ServiceId;
                    var item = context.Services.FirstOrDefault(x => x.ServiceId == selectedItemId);

                    if (item != null)
                    {
                        context.Services.Remove(item);
                        context.SaveChanges();
                    }
                    listServices.ItemsSource = null;
                    LoadListView();
                }
                else
                {
                    MessageBox.Show("Выберите услугу");
                }

            }
        }

        private void AddItemToOrder()
        {
            foreach (var selectedItem in listServices.SelectedItems)
            {
                Service service = selectedItem as Service;
                if (service != null)
                {
                    selectedServices.Add(service);
                    MessageBox.Show($"Товар с id: {service.ServiceId} и названием: {service.ServiceName}\n Был добавлен в корзину");
                }
                else
                {
                    MessageBox.Show("Выберите услугу");
                }
            }
        }

        private void EditItem()
        {
            var selectedItem = listServices.SelectedItem;

            using (DataModel context = new DataModel())
            {
                if (selectedItem != null)
                {
                    int selectedItemId = ((Service)selectedItem).ServiceId;
                    var item = context.Services.FirstOrDefault(x => x.ServiceId == selectedItemId);

                    EditWindow editWindow = new EditWindow(item);

                    editWindow.Closing += (sende1r, ex) =>
                    {
                        listServices.ItemsSource = null;
                        LoadListView();
                    };

                    editWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Выберите услугу");
                }
            }
        }

        private void AddItem()
        {
            EditWindow editWindow = new EditWindow(null);
            editWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadListView();
            LoadCbCategories();
            LoadCbPrice();
            LoadCbDiscount();
        }

        private void btnAddService_Click(object sender, RoutedEventArgs e)
        {
            AddItem();
        }

        private void btnEditService_Click(object sender, RoutedEventArgs e)
        {
            EditItem();
        }

        private void btnDeleteService_Click(object sender, RoutedEventArgs e)
        {
            DeleteItem();
        }

        private void cbDiscount_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            listServices.ItemsSource = null;
            LoadListView();
        }

        private void cbPrice_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            listServices.ItemsSource = null;
            LoadListView();
        }

        private void cbCategory_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            listServices.ItemsSource = null;
            LoadListView();
        }

        private void tbSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            listServices.ItemsSource = null;
            LoadListView();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddItemToOrder();
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            OrderWindow orderWindow = new OrderWindow(selectedServices, customer);

            orderWindow.Closing += (sende1r, ex) =>
            {
                selectedServices.Clear();
            };
            orderWindow.ShowDialog();

        }
    }
}
