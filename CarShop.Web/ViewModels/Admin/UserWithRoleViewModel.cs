namespace CarShop.Web.ViewModels.Admin
{
    public class UserWithRoleViewModel
    {
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? CurrentRole { get; set; }
        public List<string>? AllRoles { get; set; }
    }
}
