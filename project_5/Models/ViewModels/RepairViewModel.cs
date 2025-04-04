using System.ComponentModel.DataAnnotations;

namespace project_5.Models.ViewModels
{
    public class RepairViewModel
    {
        public int CarId { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public int Price { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

    }

}
