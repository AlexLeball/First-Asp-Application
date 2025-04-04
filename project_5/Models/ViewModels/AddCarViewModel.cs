using project_5.Models.Entities;
using System.Security.Policy;

namespace project_5.Models.ViewModels
{
    public class AddCarViewModel
    {
        public int BrandId { get; set; }
        public int CarModelId { get; set; }
        public int Year { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime AvailableDate { get; set; }
        public string UrlPhoto { get; set; }
        public string Finition { get; set; }
        public List<Repair> Repairs { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
        public List<CarModel> CarModels { get; set; } = new();
    }
}
