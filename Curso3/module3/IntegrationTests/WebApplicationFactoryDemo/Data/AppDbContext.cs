using Microsoft.EntityFrameworkCore;
using WebApplicationFactoryDemo.Models;

namespace WebApplicationFactoryDemo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
}