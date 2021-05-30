using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Daos;
using Poputi.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Poputi.Logic.Services
{
    public class RoutesService : IRoutesService
    {
        private readonly MainContext _mainContext;

        public RoutesService(MainContext mainContext)
        {
            _mainContext = mainContext;
        }

        public async ValueTask AddTravellerRouteAsync(CityRoute cityRoute, CancellationToken cancellationToken = default)
        {
            await _mainContext.CityRoutes.AddAsync(cityRoute, cancellationToken).ConfigureAwait(false);
            await _mainContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async ValueTask AddDriverRouteAsync(CityRoute cityRoute, CancellationToken cancellationToken = default)
        {
            await _mainContext.CityRoutes.AddAsync(cityRoute, cancellationToken).ConfigureAwait(false);
            await _mainContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Примерррр.
        /// </summary>
        public void Example()
        {
            var seattle = new Point(-122.333056, 47.609722) { SRID = 4326 };
            var redmond = new Point(-122.123889, 47.669444) { SRID = 4326 };

            // In order to get the distance in meters, we need to project to an appropriate
            // coordinate system. In this case, we're using SRID 2855 since it covers the geographic
            // area of our data
            var distanceInDegrees = seattle.Distance(redmond);
            var distanceInMeters = seattle.ProjectTo(2855).Distance(redmond.ProjectTo(2855));
        }

        /// <summary>
        /// Мы в говне.
        /// </summary>
        /// <param name="cityRoute"> </param>
        /// <param name="distance"> Расстояние в метрах </param>
        /// <returns> </returns>
        public IAsyncEnumerable<CityRoute> FindNotMatchedRoutesWithinAsync(CityRoute cityRoute, double distance, double timeDiff = 15)
        {
            return _mainContext.CityRoutes.Include(p => p.User).AsQueryable().Where(x
                => x.Start.IsWithinDistance(cityRoute.Start, distance)
                && x.End.IsWithinDistance(cityRoute.End, distance)
                && (x.DateTime - cityRoute.DateTime < TimeSpan.FromMinutes(timeDiff) || x.DateTime - cityRoute.DateTime >= TimeSpan.FromMinutes(timeDiff))
                //&& x.CityRouteType == DataAccess.Enums.CityRouteType.ByDriver
                && !x.IsMatched
                ).ToAsyncEnumerable();
        }

        public async ValueTask MatchRoutesAsync(CityRoute cityRoute)
        {
            var cityRouteEntry = await _mainContext.CityRoutes.FindAsync(cityRoute.Id);
            cityRouteEntry.IsMatched = true;
            //await _mainContext.RouteMatches.AddAsync(new RouteMatch { RouteMatchType = DataAccess.Enums.RouteMatchType.WithDriver, MatchedCityRoute = cityRouteEntry, FellowTravelers = new[] { fellow } });
            await _mainContext.SaveChangesAsync();
        }
    }
}
