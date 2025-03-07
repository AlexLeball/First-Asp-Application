namespace project_5.Models.Entities
{
    public class CarModel
    {
        int Id { get; set; }
        string? Model { get; set; }

        public virtual ICollection<Car>? Cars { get; set; }
    }
}
