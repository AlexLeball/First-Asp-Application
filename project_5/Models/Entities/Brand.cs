using System.ComponentModel.DataAnnotations;

namespace project_5.Models.Entities
{
    public class Brand
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public virtual ICollection<Car>? Cars { get; set; }
    }
}
