using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using wpf_shop.Database;

namespace wpf_shop.Windows
{
    public partial class ProductsWindow : Window
    {
        private User _loggedUser;
        private TradeEntities2 db = TradeEntities2.GetContext();
        List<Product> Products;
        List<Product> FilteredProducts;

        public ProductsWindow(User user)
        {
            InitializeComponent();
            _loggedUser = user;
            Username.Text = user.UserFullName;
            db = TradeEntities2.GetContext();
            Products = db.Product.ToList();
            FilteredProducts = Products?.ToList() ?? new List<Product>();
            ProductsListView.ItemsSource = Products;

            // Заполняем выпадающий список производителей
            LoadManufacturers();
            UpdateQuantityText(); // Обновляем текст с количеством товаров при инициализации

            // Проверяем роль пользователя
            if (_loggedUser.UserRole == 1) // 1 - Администратор
            {
                AdminButtonsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                AdminButtonsPanel.Visibility = Visibility.Collapsed;
            }
        }

        public ProductsWindow()
        {
            InitializeComponent();
            _loggedUser = null;
            Username.Text = "Гость";
            db = TradeEntities2.GetContext();
            Products = db.Product.ToList();
            FilteredProducts = Products?.ToList() ?? new List<Product>();
            ProductsListView.ItemsSource = Products;

            // Заполняем выпадающий список производителей
            LoadManufacturers();
            UpdateQuantityText(); // Обновляем текст с количеством товаров при инициализации

            AdminButtonsPanel.Visibility = Visibility.Collapsed;
        }

        // Загрузка производителей в ComboBox
        private void LoadManufacturers()
        {
            var manufacturers = db.Manufacturers.ToList();
            manufacturers.Insert(0, new Manufacturers { Name = "Все производители" }); // Добавляем пункт "Все производители"
            ManufacturerFilterCB.ItemsSource = manufacturers;
            ManufacturerFilterCB.DisplayMemberPath = "Name";
            ManufacturerFilterCB.SelectedIndex = 0; // Выбираем первый элемент по умолчанию
        }

        // Обновление текста с количеством товаров
        private void UpdateQuantityText()
        {
            int totalProducts = Products.Count;
            int displayedProducts = ProductsListView.Items.Count;
            QuantityTB.Text = $"{displayedProducts} из {totalProducts}";
        }

        // Поиск товаров по тексту
        private void UserSearchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        // Фильтрация по производителю
        private void ManufacturerFilterCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        // Применение фильтров
        private void ApplyFilters()
        {
            string searchText = UserSearchTB.Text.ToLower();
            var selectedManufacturer = ManufacturerFilterCB.SelectedItem as Manufacturers;
            if (selectedManufacturer == null) return;

            FilteredProducts = db.Product
               .Where(p =>
                   (string.IsNullOrEmpty(searchText) || // Если поисковый текст пуст, то условие выполняется
                   p.ProductName.ToLower().Contains(searchText) ||
                   p.Description.ToLower().Contains(searchText) ||
                   p.Manufacturers.Name.ToLower().Contains(searchText) ||
                   p.ProductCost.ToString().Contains(searchText) ||
                   p.QuantityWarehouse.ToString().Contains(searchText))
               )
               .Where(p =>
                   selectedManufacturer.Name == "Все производители" || // Если выбран "Все производители", то условие выполняется
                   p.Manufacturers.Name == selectedManufacturer.Name // Иначе фильтруем по выбранному производителю
               )
               .ToList();

            ProductsListView.ItemsSource = FilteredProducts;
            UpdateQuantityText(); // Обновляем текст с количеством товаров после фильтрации
        }

        // Сортировка по возрастанию стоимости
        private void SortAscendingClick(object sender, RoutedEventArgs e)
        {
            FilteredProducts = FilteredProducts.OrderBy(p => p.ProductCost).ToList();
            ProductsListView.ItemsSource = FilteredProducts;
            UpdateQuantityText(); // Обновляем текст с количеством товаров после сортировки
        }

        // Сортировка по убыванию стоимости
        private void SortDescendingClick(object sender, RoutedEventArgs e)
        {
            FilteredProducts = FilteredProducts.OrderByDescending(p => p.ProductCost).ToList();
            ProductsListView.ItemsSource = FilteredProducts;
            UpdateQuantityText(); // Обновляем текст с количеством товаров после сортировки
        }

        // Выход из системы
        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы вышли из системы");
            this.Close(); // Закрываем текущее окно
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            WindowAddRedact window = new WindowAddRedact();
            window.ShowDialog();
            // Обновляем список товаров после закрытия окна
            Products = db.Product.ToList();
            ProductsListView.ItemsSource = Products;
            UpdateQuantityText();
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsListView.SelectedItem as Product;
            if (selectedProduct != null)
            {
                db.Product.Remove(selectedProduct);
                db.SaveChanges();
                // Обновляем список товаров
                Products = db.Product.ToList();
                ProductsListView.ItemsSource = Products;
                UpdateQuantityText();

            }
            else
            {
                MessageBox.Show("Выберите товар для удаления.");
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = ProductsListView.SelectedItem as Product;
            if (selectedProduct != null)
            {
                WindowAddRedact window = new WindowAddRedact(selectedProduct);
                window.ShowDialog();
                // Обновляем список товаров после закрытия окна
                Products = db.Product.ToList();
                ProductsListView.ItemsSource = Products;
                UpdateQuantityText();
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования.");
            }
        }
        private List<Product> _basket = new List<Product>();

        private void ProductsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedProduct = ProductsListView.SelectedItem as Product;
            if (selectedProduct != null)
            {
                // Добавляем товар в корзину
                _basket.Add(selectedProduct);
                MessageBox.Show($"Товар \"{selectedProduct.ProductName}\" добавлен в корзину.");
            }
        }

        private void BasketButton_Click(object sender, RoutedEventArgs e)
        {
            WindowBasket basketWindow = new WindowBasket(_basket);
            basketWindow.Owner = this; // Устанавливаем текущее окно как владельца
            basketWindow.ShowDialog();
        }
    }
}