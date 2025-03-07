using System.ComponentModel.DataAnnotations;

namespace project_5.Models.Entities
{
    public class CarModel
    {
        public int Id { get; set; }
        public required string Model { get; set; }
        public virtual ICollection<Car>? Cars { get; set; }
    }
}
