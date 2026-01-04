namespace EhjozProject.Web.ViewModels.Admin
{
    public class UserManagementViewModel
    {
        public List<UserViewModel> Users { get; set; } = new();
    }

    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string City { get; set; } = null!;
        public string CreatedDate { get; set; } = null!;
        public bool IsLocked { get; set; }
    }
}

