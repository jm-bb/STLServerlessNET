using Microsoft.EntityFrameworkCore;
using STLServerlessNET.Entities.Web;

public class WebDbContext : DbContext
{
    public WebDbContext(DbContextOptions<WebDbContext> options) : base(options) { }
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Order to User (Optional Relationship)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User) // Navigation property in Order
            .WithMany(u => u.Orders) // Collection navigation property in User
            .HasForeignKey(o => o.UserId) // Foreign key in Order table
            .OnDelete(DeleteBehavior.Restrict);

        // Cart to Order (One-to-Many)
        modelBuilder.Entity<Cart>()
            .HasOne(c => c.Order) // Navigation property in Cart
            .WithMany(o => o.Carts) // Collection navigation property in Order
            .HasForeignKey(c => c.OrderId) // Foreign key in Cart table
            .OnDelete(DeleteBehavior.Restrict);

        // Cart to Product (Many-to-One)
        modelBuilder.Entity<Cart>()
            .HasOne(c => c.Product) // Navigation property in Cart
            .WithMany(p => p.Carts) // Collection navigation property in Product
            .HasForeignKey(c => c.ProdId) // Foreign key in Cart table
            .OnDelete(DeleteBehavior.Restrict);
    }
}
