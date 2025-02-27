namespace project_5.Models.Entities.cs
{
    
    public class Car
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Finition { get; set; }
        public string? UrlPhoto { get; set; }
        public DateTime AvailableDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal SalePrice { get; set; }
        public decimal PurchasePrice { get; set; }

        public virtual ICollection<Repair>? Repairs { get; set; }
    }
}

