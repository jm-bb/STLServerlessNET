using Microsoft.EntityFrameworkCore;

public class WebDbContext: DbContext
{
    public WebDbContext(DbContextOptions<WebDbContext> options) : base(options) { }

    public DbSet<Zone> Zones { get; set; }
}
