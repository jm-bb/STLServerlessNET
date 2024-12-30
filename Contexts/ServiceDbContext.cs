using Microsoft.EntityFrameworkCore;

public class ServiceDbContext : DbContext
{
    public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options) { }

    public DbSet<Carrier> Carriers { get; set; }
}
