namespace EhjozProject.Web.ViewModels.Admin
{
    public class StadiumManagementViewModel
    {
        public List<StadiumViewModel> Stadiums { get; set; } = new();
    }

    public class StadiumViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Address { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        public bool IsActive { get; set; }
        public string OwnerName { get; set; } = null!;
    }
}

