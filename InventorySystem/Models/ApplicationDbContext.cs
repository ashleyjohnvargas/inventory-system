using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public required DbSet<Product> Products { get; set; }
        public required DbSet<User> Users { get; set; }
        public required DbSet<UserProfile> UserProfiles { get; set;}
        public DbSet<ProductImage> ProductImages { get; set;}
    }
}
