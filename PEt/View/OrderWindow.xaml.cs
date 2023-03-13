using PEt.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        List<Service> selectedServices;
        User customer;

        public OrderWindow(List<Service> selectedServices, User customer)
        {
            InitializeComponent();

            this.selectedServices = selectedServices;
            this.customer = customer;

            listSelectedServices.ItemsSource = selectedServices;


            if (selectedServices != null )
            {
                decimal? totalPrice = 0;
                int amountServices = selectedServices.Count;

                foreach (Service service in selectedServices)
                {
                    if (service.ServiceDiscount != null)
                    {
                        totalPrice += service.ServicePrice * (100 - (int?)service.ServiceDiscount) / 100;
                    }
                    else
                    {
                        totalPrice += service.ServicePrice;
                    }
                }
                totalPriceServices.Text = $"Стоимость заказа со всеми скидками: {totalPrice}";
                countServices.Text = $"В корзине {amountServices} услуг";
            }

        }

        private void CreateOrder()
        {
            using (DataModel context = new DataModel())
            {
                // Создаем новый заказ
                Order order = new Order()
                {
                    OrderUserId = customer.UserId, // здесь должен быть ID пользователя, создающего заказ
                    OrderDate = DateTime.Now,
                    OrderStatus = "Новый", // статус заказа по умолчанию
                    PaymentStatus = "Оплачен", // статус оплаты по умолчанию
                };
                context.Orders.Add(order);
                context.SaveChanges();

                // Получаем идентификатор созданного заказа
                int orderId = order.OrderId;

                // Добавляем выбранные товары в заказ
                int? total = 0;
                foreach (Service service in selectedServices)
                {
                    // Создаем новую запись в таблице "OrderServices"
                    OrderService orderService = new OrderService()
                    {
                        OrderId = orderId,
                        ServiceId = service.ServiceId,
                    };
                    context.OrderServices.Add(orderService);

                    // Добавляем стоимость текущего товара к общей стоимости заказа
                    if (service.ServiceDiscount != null)
                    {
                        total += service.ServicePrice * (100 - (int?)service.ServiceDiscount) / 100;
                    }
                    else
                    {
                        total += service.ServicePrice;
                    }
                }

                // Обновляем общую стоимость заказа в таблице "Orders"
                Order currentOrder = context.Orders.FirstOrDefault(o => o.OrderId == orderId);
                currentOrder.OrderTotal = (decimal)total;
                context.SaveChanges();

                MessageBox.Show("Заказ успешно создан!");
                Thread.Sleep(500);
                Close();
            }
        }

        private void DeleteFromOrder()
        {
            var selectedItem = listSelectedServices.SelectedItem as Service;
            if (selectedItem != null)
            {
                selectedServices.Remove(selectedItem);
                listSelectedServices.ItemsSource = null;
                listSelectedServices.ItemsSource = selectedServices;

                decimal totalPrice = 0;
                int amountServices = selectedServices.Count;

                foreach (Service service in selectedServices)
                {
                    totalPrice += service.ServicePrice;
                }
                totalPriceServices.Text = totalPrice.ToString();
                countServices.Text = $"В корзине {amountServices} услуг";
            }
        }

        private void ButtonDeleteFromOrder_Click(object sender, RoutedEventArgs e)
        {
            DeleteFromOrder();
        }

        private void btnCreateOrder_Click(object sender, RoutedEventArgs e)
        {
            if (selectedServices != null)
            {
                CreateOrder();
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
