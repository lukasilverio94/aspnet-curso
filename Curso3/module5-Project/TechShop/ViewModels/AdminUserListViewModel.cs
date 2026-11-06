namespace TechShop.ViewModels;

public class AdminUserListViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    /// <summary>True when the account is currently locked out (cannot sign in).</summary>
    public bool IsLockedOut { get; set; }
    public IList<string> Roles { get; set; } = [];
}
