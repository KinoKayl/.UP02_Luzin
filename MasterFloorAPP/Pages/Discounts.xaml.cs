using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MasterFloorAPP.Pages
{
    public partial class Discounts : Page
    {
        private readonly Entities db; 

        public Discounts()
        {
            InitializeComponent();
            db = new Entities(); 
            LoadDiscounts(); 
        }

        // Метод для загрузки скидок по партнерам
        private void LoadDiscounts()
        {
            // Группируем данные по партнерам и считаем общее количество товаров по каждому партнеру
            var partnerSales = db.Partner_products
                .GroupBy(p => p.Partner) 
                .Select(g => new
                {
                    Partner = g.Key, 
                    TotalQuantity = g.Sum(p => p.Quantity ?? 0) 
                })
                .ToList(); 

            // Для каждого партнера рассчитываем скидку на основе общего количества товаров
            var partnerDiscounts = partnerSales.Select(ps => new
            {
                Partner = db.Partners.FirstOrDefault(p => p.ID == ps.Partner), 
                TotalQuantity = ps.TotalQuantity, 
                Discount = CalculateDiscount(ps.TotalQuantity) 
            }).ToList(); 

            
            ListUser.ItemsSource = partnerDiscounts;
        }

        // Метод для расчета скидки в зависимости от общего количества товаров
        private decimal CalculateDiscount(int totalQuantity)
        {
           
            if (totalQuantity <= 10000)
                return 0; 
            else if (totalQuantity <= 50000)
                return 5; 
            else if (totalQuantity <= 300000)
                return 10; 
            else
                return 15; 
        }

        private void BtnAdd_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddPartner(null));
        }

        private void BtnShowSales_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ListUser.SelectedItem != null) 
            {
                
                var selectedPartner = (dynamic)ListUser.SelectedItem;
                int partnerId = selectedPartner.Partner.ID;

                NavigationService?.Navigate(new Sales(partnerId));
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите партнёра из списка.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ListUser_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            if (ListUser.SelectedItem != null)
            {
                var selectedPartner = (dynamic)ListUser.SelectedItem;
                var partner = selectedPartner.Partner;

                NavigationService?.Navigate(new AddPartner(partner));
            }
        }
    }
}