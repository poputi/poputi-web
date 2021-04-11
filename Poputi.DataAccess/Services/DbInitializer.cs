using NetTopologySuite.Geometries;
using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Daos;
using System.Linq;

namespace Poputi.DataAccess.Services
{
    public static class DbInitializer
    {
        public static void Initialize(MainContext context)
        {
            context.Database.EnsureCreated();

            // Look for any users.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            // Мок городов.
            var cities = new City[]
            {
                new City { Name = "Екатеринбург", Location = new Point (60.612778, 56.835556) },
            };
            foreach (var c in cities)
            {
                context.Cities.Add(c);
            }
            context.SaveChanges();

            // Мок пользователей.
            var user = new User[]
            {
                new User { FirstMidName = "Zachery", LastName = "M'cowis", Login = "zmcowis0", HomeCity = cities[0] },
                new User { FirstMidName = "Aristotle", LastName = "Noddle", Login = "anoddle1", HomeCity = cities[0] },
                new User { FirstMidName = "Kevyn", LastName = "Grigoli", Login = "kgrigoli2", HomeCity = cities[0] },
                new User { FirstMidName = "Aleda", LastName = "Prynne", Login = "aprynne3", HomeCity = cities[0] },
                new User { FirstMidName = "Helsa", LastName = "Seys", Login = "hseys4", HomeCity = cities[0] },
                new User { FirstMidName = "Ginnie", LastName = "Dmitrievski", Login = "gdmitrievski5", HomeCity = cities[0] },
                new User { FirstMidName = "Lonnard", LastName = "Osborn", Login = "losborn6", HomeCity = cities[0] },
                new User { FirstMidName = "Rowen", LastName = "Sedgmond", Login = "rsedgmond7", HomeCity = cities[0] },
                new User { FirstMidName = "Linnie", LastName = "Roffey", Login = "lroffey8", HomeCity = cities[0] },
                new User { FirstMidName = "Danika", LastName = "Chown", Login = "dchown9", HomeCity = cities[0] },
                new Driver {FirstMidName = "Adolf", LastName = "Gebe", Login = "kerner1", HomeCity = cities[0]}
            };
            foreach (var u in user)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();

            // Мок маршрутов.
            var routes = new CityRoute[]
            {
                new CityRoute { UserId = 1, Start = new Point(27.8428651, -22.0028763), End = new Point(-74.8745545, -10.9265815) },
                new CityRoute { UserId = 1, Start = new Point(23.9452683, 54.632212), End = new Point(18.2763069, 6.5060695) },
                new CityRoute { UserId = 1, Start = new Point(67.068153, 33.731926), End = new Point(117.647093, 24.513025) },
                new CityRoute { UserId = 3, Start = new Point(123.3427328, 41.6528112), End = new Point(-82.4102628, 22.9861757) },
                new CityRoute { UserId = 5, Start = new Point(45.9860099, 43.2496743), End = new Point(116.391507, 37.627661) },
                new CityRoute { UserId = 5, Start = new Point(-9.1663731, 29.7755232), End = new Point(107.0037509, -7.2693645) },
                new CityRoute { UserId = 6, Start = new Point(123.1618753, -8.4324536), End = new Point(2.2289339, 48.8706152) },
                new CityRoute { UserId = 6, Start = new Point(23.9333037, 37.8833564), End = new Point(-51.1787796, -29.1556093) },
                new CityRoute { UserId = 6, Start = new Point(113.5516516, -8.2174219), End = new Point(18.78031, 52.671181) },
                new CityRoute { UserId = 7, Start = new Point(-80.4437781, 22.1599753), End = new Point(112.723615, 27.754532) },
            };
            foreach (var r in routes)
            {
                context.CityRoutes.Add(r);
            }
            context.SaveChanges();
        }
    }
}
