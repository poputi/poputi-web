using Microsoft.EntityFrameworkCore;
using Poputi.DataAccess.Daos;

namespace Poputi.DataAccess.Contexts
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<CityRoute> CityRoutes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Установка постгиса.
            modelBuilder.HasPostgresExtension("postgis");
        }
    }
}
