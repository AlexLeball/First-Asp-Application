using project_5.Models.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace project_5.Models.ViewModels
{
    public class AddCarViewModel
    {
        [Required(ErrorMessage = "La marque est requise.")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Le modèle de voiture est requis.")]
        public int CarModelId { get; set; }

        [Required(ErrorMessage = "L'année est requise.")]
        [Range(1990, 2100, ErrorMessage = "L'année doit être comprise entre 1990 et 2100.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Le prix d'achat est requis.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le prix d'achat doit être un nombre positif.")]
        public decimal PurchasePrice { get; set; }

        [Required(ErrorMessage = "Le prix de vente est requis.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le prix de vente doit être un nombre positif.")]
        public decimal SalePrice { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "La date de disponibilité est requise.")]
        public DateTime AvailableDate { get; set; }

        [Required(ErrorMessage = "L'URL de la photo est requise.")]
        public string UrlPhoto { get; set; }

        [Required(ErrorMessage = "La photo de la voiture est requise.")]
        public IFormFile CarPhotoFile { get; set; }

        [StringLength(100, ErrorMessage = "La finition ne peut pas dépasser 100 caractères.")]
        public string Finition { get; set; }

        public List<Repair>? Repairs { get; set; } = new();
        public List<Brand> Brands { get; set; } = new();
        public List<CarModel> CarModels { get; set; } = new();
    }
}
