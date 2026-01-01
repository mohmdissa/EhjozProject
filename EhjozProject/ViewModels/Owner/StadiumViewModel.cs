using System.ComponentModel.DataAnnotations;

namespace EhjozProject.Web.ViewModels.Owner
{
    public class StadiumViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Stadium name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Stadium Name")]
        public string Name { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "City is required")]
        [Display(Name = "City")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Price per hour is required")]
        [Range(1, 10000, ErrorMessage = "Price must be between $1 and $10,000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price Per Hour ($)")]
        public decimal PricePerHour { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;
    }
}