namespace project_5.Models.Entities.cs
{
    public class CarModel
    {
        int Id { get; set; }
        string? ModelType { get; set; }

        public virtual ICollection<Car>? Cars { get; set; }
    }
}
