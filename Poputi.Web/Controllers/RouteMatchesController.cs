using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Daos;
using Poputi.Web.Models;

namespace Poputi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteMatchesController : ControllerBase
    {
        private readonly MainContext _context;

        private MapperConfiguration _mapper = new MapperConfiguration(cfg =>
       {
           cfg.CreateMap<RouteMatchModel, RouteMatch>()
           .ForMember(x => x.FellowTravelers, y => y.MapFrom(src => src.FellowTravelers.Select(ft => new User { Id = ft })))
           .ForMember(x => x.MatchedCityRoute, y => y.MapFrom(src => new CityRoute { Id = src.MatchedCityRoute }));
           //cfg.CreateMap<RouteMatch, RouteMatchModel>();
       });

        public RouteMatchesController(MainContext context)
        {
            _context = context;
        }

        // GET: api/RouteMatches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RouteMatch>>> GetRouteMatches()
        {
            return await _context.RouteMatches.Include(x => x.MatchedCityRoute).Include(x => x.FellowTravelers).ToListAsync();
        }

        // GET: api/RouteMatches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RouteMatch>> GetRouteMatch(Guid id)
        {
            var routeMatch = await _context.RouteMatches.FindAsync(id);

            if (routeMatch == null)
            {
                return NotFound();
            }

            return routeMatch;
        }

        // PUT: api/RouteMatches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRouteMatch(Guid id, RouteMatch routeMatch)
        {
            if (id != routeMatch.Id)
            {
                return BadRequest();
            }

            _context.Entry(routeMatch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RouteMatchExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RouteMatches
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RouteMatch>> PostRouteMatch(RouteMatchModel routeMatch)
        {
            var match = _mapper.CreateMapper().Map<RouteMatchModel, RouteMatch>(routeMatch);

            var route = await _context.CityRoutes.FindAsync(routeMatch.MatchedCityRoute);
            match.MatchedCityRoute = route;

            var fellowTravalers = _context.Users.AsQueryable().Where(x => routeMatch.FellowTravelers.Contains(x.Id));
            match.FellowTravelers = await fellowTravalers.ToListAsync();

            _context.RouteMatches.Add(match);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/RouteMatches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRouteMatch(Guid id)
        {
            var routeMatch = await _context.RouteMatches.FindAsync(id);
            if (routeMatch == null)
            {
                return NotFound();
            }

            _context.RouteMatches.Remove(routeMatch);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RouteMatchExists(Guid id)
        {
            return _context.RouteMatches.Any(e => e.Id == id);
        }
    }
}
