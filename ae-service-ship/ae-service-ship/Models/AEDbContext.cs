using Microsoft.EntityFrameworkCore;

namespace ae_service_ship.Models
{
    public class AEDbContext : DbContext
    {
        public AEDbContext(DbContextOptions<AEDbContext> options) : base(options) { }

        public DbSet<Port> Ports { get; set; }
        public DbSet<Ship> Ships { get; set; }
    }
}
