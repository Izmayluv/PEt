using System;
using System.Linq;
using System.Windows;
using PEt.Entity;

namespace PEt
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string MessageBoxText = "Пользователь не найден";

        public MainWindow()
        {
            InitializeComponent();
        }

        User customer;

        private void Login(string login, string password)
        {
            using (DataModel dataModel = new DataModel())
            {
                bool authenticated = false;

                try
                {
                    customer = dataModel.Users.
                    Where(x => x.UserPhoneNumber == login && x.UserPassword == password).
                    FirstOrDefault();

                    if (customer != null) { authenticated = true; }
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show(ex.Message);
                }
                

                if (authenticated)
                {
                    CatalogWindow catalogWindow = new CatalogWindow(customer);
                    switch (customer.UserRole)
                    {
                        default:
                            break;
                        case 1:
                            catalogWindow.Show();
                            this.Hide();
                            break;
                        case 2:
                            catalogWindow.Show();
                            this.Hide();
                            break;

                        case 3:
                            catalogWindow.btnAddService.Visibility = Visibility.Hidden;
                            catalogWindow.btnCreateOrder.Visibility = Visibility.Visible;
                            catalogWindow.btnDeleteService.Visibility = Visibility.Hidden;
                            catalogWindow.btnEditService.Visibility = Visibility.Hidden;
                            catalogWindow.Show();
                            this.Hide();
                            break;
                    }                    
                }

                if (!authenticated)
                {
                    _ = MessageBox.Show(MessageBoxText);
                    return;
                }

            }
        }

        private void EnterLikeGuest()
        {
            CatalogWindow catalogWindow = new CatalogWindow(customer);
            catalogWindow.btnAddService.Visibility = Visibility.Hidden;
            catalogWindow.btnCreateOrder.Visibility = Visibility.Hidden;
            catalogWindow.btnDeleteService.Visibility = Visibility.Hidden;
            catalogWindow.btnEditService.Visibility = Visibility.Hidden;
            catalogWindow.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login(tbLogin.Text, tbPassword.Password);
        }

        private void labelRegistration_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
        }

        private void labelEnterLikeGuest_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EnterLikeGuest();
        }

        private void labelPasswordRecovery_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //
        }
    }
}
