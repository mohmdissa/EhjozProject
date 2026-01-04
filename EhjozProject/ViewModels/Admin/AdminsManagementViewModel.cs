using System.ComponentModel.DataAnnotations;

namespace EhjozProject.Web.ViewModels.Admin
{
    public class AdminsManagementViewModel
    {
        public List<AdminUserViewModel> Admins { get; set; } = new();
        public CreateAdminViewModel CreateAdmin { get; set; } = new();
    }

    public class AdminUserViewModel
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? City { get; set; }
    }

    public class CreateAdminViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        public string? FullName { get; set; }
        public string? City { get; set; }
    }
}


