using TechShop.Models;

namespace TechShop.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalOrders { get; set; }
    public int TotalUsers { get; set; }
    public IReadOnlyList<Order> RecentOrders { get; set; } = [];
}
