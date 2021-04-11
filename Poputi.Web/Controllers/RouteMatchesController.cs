using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Poputi.Web.Controllers
{
    [Route("api/[controller]")]
    public class RouteMatchesController : ControllerBase
    {
        private readonly MainContext _mainContext;
        private readonly ILogger<RouteMatchesController> _logger;

        public RouteMatchesController(MainContext mainContext, ILogger<RouteMatchesController> logger)
        {
            _mainContext = mainContext;
            _logger = logger;
        }

        // Post
        // Get
        // Put
        // Delete
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async ValueTask<ActionResult<RouteMatch>> PostRouteMatch(RouteMatch routeMatch, CancellationToken cancellationToken)
        {
            if ((await _mainContext.RouteMatches.FindAsync(routeMatch.MatchedCityRoute.Id, cancellationToken)) is not null)
            {
                return BadRequest("Маршрут уже занят");
            }
            var cityRoute = await _mainContext.CityRoutes.FindAsync(new object[] { routeMatch.MatchedCityRoute.Id }, cancellationToken);
            if (cityRoute is null)
            {
                return BadRequest("Маршрута не существует");
            }
            routeMatch.MatchedCityRoute = cityRoute;
            if (cityRoute.CityRouteType == DataAccess.Enums.CityRouteType.ByDriver)
            {
                // FIXME: а пользователь правда `Driver`?
                //routeMatch.Driver = cityRoute.User as Driver;
                routeMatch.RouteMatchType = DataAccess.Enums.RouteMatchType.WithDriver;
                var entry = await _mainContext.RouteMatches.AddAsync(routeMatch, cancellationToken);
                await _mainContext.SaveChangesAsync();
                routeMatch = entry.Entity;
                return CreatedAtAction("GetRouteMatch", new { id = routeMatch.Id }, routeMatch);
            }
            //routeMatch.Driver = cityRoute.User as Driver;
            //routeMatch.RouteMatchType = DataAccess.Enums.RouteMatchType.WithDriver;
            //var entry = await _mainContext.RouteMatches.AddAsync(routeMatch, cancellationToken);
            //await _mainContext.SaveChangesAsync();
            //routeMatch = entry.Entity;
            //return CreatedAtAction("GetRouteMatch", new { id = routeMatch.Id }, routeMatch);
            return BadRequest();
        }
    }
}
