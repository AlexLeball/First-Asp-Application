namespace project_5.Models.Entities.cs
{
    
    public class Car
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string? Finition { get; set; }
        public string? UrlPhoto { get; set; }
        public DateTime AvailableDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        //has a navigation property to the Repair table
        public virtual ICollection<Repair>? Repairs { get; set; }
        //has a foreign key to the CarModel table
        public int ModelId { get; set; }
        public virtual CarModel ModelType { get; set; }
        //has a foreign key to the Brand table
        public int BrandId { get; set; }
        public virtual Brand BrandName { get; set; }
    }
}

