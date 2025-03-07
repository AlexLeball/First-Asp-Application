namespace project_5.Models.Entities
{
    public class Brand
    {
        int Id { get; set; }
        string? Name { get; set; }
        public virtual ICollection<Car>? Cars { get; set; }
    }
}
