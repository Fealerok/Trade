using System;
using System.Collections.Generic;
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
using wpf_shop.Database;
using wpf_shop.Windows;

namespace wpf_shop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isUnsuccessfully = false;
        private TradeEntities2 db;
        public MainWindow()
        {
            InitializeComponent();
            db = TradeEntities2.GetContext();
        }

        private void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTB.Text;
            string password = PasswordTB.Text;

            if (login == string.Empty || password == string.Empty)
            {
                MessageBox.Show("не все поля заполнены");
            }

            else
            {
                if (_isUnsuccessfully)
                {
                    captcha captcha = new captcha();
                    captcha.ShowDialog();

                    if (!captcha.isCompleted)
                    {
                        MessageBox.Show("Для продолжения авторизации необходимо пройти капчу");
                        return;
                    }

                }

                List<User> foundUserList = db.User.Where(user => user.UserLogin == login && user.UserPassword == password).ToList();

                if (foundUserList.Count <= 0)
                {
                    MessageBox.Show("Пользователь не найден");
                    _isUnsuccessfully = true;
                }

                else
                {
                    User foundUser = foundUserList[0]; 
                    ProductsWindow window = new ProductsWindow(foundUser);
                    window.Show();
                    this.Close();
                }
            }
        }

        private void GuestBTN_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow window = new ProductsWindow();
            window.Show();
            this.Close();
        }
    }
}
