using project_5.Models.Entities;

namespace project_5.Models.ViewModels
{
    public class EditCarViewModel
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public int CarModelId { get; set; }
        public int Year { get; set; }
        public decimal SalePrice { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public string UrlPhoto { get; set; }
        public string Finition { get; set; }
        public List<Brand> Brands { get; set; } = new();
        public List<CarModel> CarModels { get; set; } = new();
    }
}

