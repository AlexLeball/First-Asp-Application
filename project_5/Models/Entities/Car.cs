using System.ComponentModel.DataAnnotations;

namespace project_5.Models.Entities
{
    
    public class Car
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public required string Finition { get; set; }
        public required string UrlPhoto { get; set; }
        public DateTime AvailableDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        //has a navigation property to the Repair table
        public virtual ICollection<Repair>? Repairs { get; set; }
        //has a foreign key to the CarModel table
        public required virtual CarModel CarModel { get; set; }
        //has a foreign key to the Brand table
        public required virtual Brand Brand { get; set; }
    }
}

