using Microsoft.EntityFrameworkCore;

namespace project_5.Models.Entities.cs
{
    public class EFCoreDbContext : DbContext
    {
        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options) : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Repair> Repairs { get; set; }
    }
}
