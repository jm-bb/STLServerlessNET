using Microsoft.EntityFrameworkCore;
using STLServerlessNET.Entities.Service;

public class ServiceDbContext : DbContext
{
    public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options) { }

    public DbSet<Carrier> Carriers { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
