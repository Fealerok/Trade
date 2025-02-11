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
using System.Windows.Shapes;
using wpf_shop.Database;

namespace wpf_shop.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowBasket.xaml
    /// </summary>
    public partial class WindowBasket : Window
    {
        private List<Product> _basket;

        public WindowBasket(List<Product> basket)
        {
            InitializeComponent();
            _basket = basket;
            BasketListView.ItemsSource = _basket; // Отображаем товары в корзине
        }

        // Обработчик нажатия на кнопку "Оформить заказ"
        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_basket.Count > 0)
            {
                MessageBox.Show("Заказ оформлен успешно!");
                _basket.Clear(); 
                this.Close();
            }
            else
            {
                MessageBox.Show("Корзина пуста. Добавьте товары перед оформлением заказа.");
            }
        }

        private void BasketListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedProduct = BasketListView.SelectedItem as Product;
            if (selectedProduct != null)
            {
                // Удаляем товар из корзины
                _basket.Remove(selectedProduct);
                // Обновляем отображение списка товаров
                BasketListView.ItemsSource = null; 
                BasketListView.ItemsSource = _basket; 
            }
        }
    }
}
