using System.ComponentModel.DataAnnotations;

namespace project_5.Models.Entities
{
    public class Repair
    {
        public int Id { get; set; }
        public required int Price { get; set; }
        public required virtual Car Car { get; set; }
        public required string Description { get; set; }

    }
}
