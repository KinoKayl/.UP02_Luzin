using System.Linq;
using System.Windows.Controls;

namespace MasterFloorAPP.Pages
{
    public partial class Sales : Page
    {
        private readonly Entities db;
        public Sales(int partnerID)
        {
            InitializeComponent();
            db = new Entities();
            
            var partnerSales = db.Partner_products.Where(p => p.Partner == partnerID).ToList();

            DataGridSales.ItemsSource = partnerSales;
        }
    }
}
