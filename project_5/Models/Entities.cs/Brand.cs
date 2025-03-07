namespace project_5.Models.Entities.cs
{
    public class Brand
    {
        int Id { get; set; }
        string? BrandName { get; set; }
        public virtual ICollection<Car>? Cars { get; set; }
    }
}
