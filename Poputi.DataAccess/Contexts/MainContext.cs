using Microsoft.EntityFrameworkCore;
using Poputi.DataAccess.Daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.DataAccess.Contexts
{
    public class MainContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<CityRoute> CityRoutes { get; set; }
        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {

        }
    }
}
