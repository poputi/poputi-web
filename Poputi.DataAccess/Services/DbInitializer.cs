using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var user = new User[]
            {
                new User { FirstMidName = "Zachery", LastName = "M'cowis", Login = "zmcowis0" },
                new User { FirstMidName = "Aristotle", LastName = "Noddle", Login = "anoddle1" },
                new User { FirstMidName = "Kevyn", LastName = "Grigoli", Login = "kgrigoli2" },
                new User { FirstMidName = "Aleda", LastName = "Prynne", Login = "aprynne3" },
                new User { FirstMidName = "Helsa", LastName = "Seys", Login = "hseys4" },
                new User { FirstMidName = "Ginnie", LastName = "Dmitrievski", Login = "gdmitrievski5" },
                new User { FirstMidName = "Lonnard", LastName = "Osborn", Login = "losborn6" },
                new User { FirstMidName = "Rowen", LastName = "Sedgmond", Login = "rsedgmond7" },
                new User { FirstMidName = "Linnie", LastName = "Roffey", Login = "lroffey8" },
                new User { FirstMidName = "Danika", LastName = "Chown", Login = "dchown9" },
            };
            foreach (var u in user)
            {
                context.Users.Add(u);
            }
            context.SaveChanges();

            var routes = new CityRoute[]
            {
                new CityRoute { UserId = 1,StartLongitude = 27.8428651, StartLatitude = -22.0028763, EndLongitude = -74.8745545, EndLatitude = -10.9265815 },
                new CityRoute { UserId = 1,StartLongitude = 23.9452683, StartLatitude = 54.632212, EndLongitude = 18.2763069, EndLatitude = 6.5060695 },
                new CityRoute { UserId = 1,StartLongitude = 67.068153, StartLatitude = 33.731926, EndLongitude = 117.647093, EndLatitude = 24.513025 },
                new CityRoute { UserId = 3,StartLongitude = 123.3427328, StartLatitude = 41.6528112, EndLongitude = -82.4102628, EndLatitude = 22.9861757 },
                new CityRoute { UserId = 5,StartLongitude = 45.9860099, StartLatitude = 43.2496743, EndLongitude = 116.391507, EndLatitude = 37.627661 },
                new CityRoute { UserId = 5,StartLongitude = -9.1663731, StartLatitude = 29.7755232, EndLongitude = 107.0037509, EndLatitude = -7.2693645 },
                new CityRoute { UserId = 6,StartLongitude = 123.1618753, StartLatitude = -8.4324536, EndLongitude = 2.2289339, EndLatitude = 48.8706152 },
                new CityRoute { UserId = 6,StartLongitude = 23.9333037, StartLatitude = 37.8833564, EndLongitude = -51.1787796, EndLatitude = -29.1556093 },
                new CityRoute { UserId = 6,StartLongitude = 113.5516516, StartLatitude = -8.2174219, EndLongitude = 18.78031, EndLatitude = 52.671181 },
                new CityRoute { UserId = 7,StartLongitude = -80.4437781, StartLatitude = 22.1599753, EndLongitude = 112.723615, EndLatitude = 27.754532 },
            };
            foreach (var r in routes)
            {
                context.CityRoutes.Add(r);
            }
            context.SaveChanges();
        }
    }
}
