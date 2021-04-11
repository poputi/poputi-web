using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Daos;
using Poputi.Logic.Interfaces;

namespace Poputi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityRoutesController : ControllerBase
    {
        private readonly MainContext _context;
        private readonly IRoutesService _routesService;

        public CityRoutesController(MainContext context, IRoutesService routesService)
        {
            _context = context;
            _routesService = routesService;
        }

        // GET: api/CityRoutes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityRoute>>> GetCityRoutes(CancellationToken cancellationToken)
        {
            return await _context.CityRoutes.AsQueryable().ToListAsync(cancellationToken);
        }

        [HttpPost("{distance}")]
        [ProducesResponseType(statusCode: 200)]
        public async ValueTask<ActionResult<List<CityRoute>>> GetCityRoutes([FromBody] CityRoute cityRoute, double distance, CancellationToken cancellationToken)
        {
            return await _routesService.FindNotMatchedRoutesWithinAsync(cityRoute, distance).ToListAsync(cancellationToken);
        }

        // GET: api/CityRoutes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CityRoute>> GetCityRoute(int id)
        {
            var cityRoute = await _context.CityRoutes.FindAsync(id);
            return cityRoute == null ? NotFound() : cityRoute;
        }

        // PUT: api/CityRoutes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCityRoute(int id, CityRoute cityRoute)
        {
            if (id != cityRoute.Id)
            {
                return BadRequest();
            }

            _context.Entry(cityRoute).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!CityRouteExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/CityRoutes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async ValueTask<ActionResult<CityRoute>> PostCityRoute(CityRoute cityRoute, CancellationToken cancellationToken)
        {
            await _routesService.AddDriverRouteAsync(cityRoute, cancellationToken);
            return CreatedAtAction("GetCityRoute", new { id = cityRoute.Id }, cityRoute);
        }

        // DELETE: api/CityRoutes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCityRoute(int id)
        {
            var cityRoute = await _context.CityRoutes.FindAsync(id);
            if (cityRoute == null)
            {
                return NotFound();
            }

            _context.CityRoutes.Remove(cityRoute);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityRouteExists(int id)
        {
            return _context.CityRoutes.Any(e => e.Id == id);
        }
    }
}
