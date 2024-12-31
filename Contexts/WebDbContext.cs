using Microsoft.EntityFrameworkCore;
using STLServerlessNET.Entities.Web;

public class WebDbContext: DbContext
{
    public WebDbContext(DbContextOptions<WebDbContext> options) : base(options) { }
    public DbSet<Zone> Zones { get; set; }
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
