using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Daos;

namespace Poputi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityRoutesController : ControllerBase
    {
        private readonly MainContext _context;

        public CityRoutesController(MainContext context)
        {
            _context = context;
        }

        // GET: api/CityRoutes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityRoute>>> GetCityRoutes()
        {
            return await _context.CityRoutes.ToListAsync();
        }

        // GET: api/CityRoutes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CityRoute>> GetCityRoute(int id)
        {
            var cityRoute = await _context.CityRoutes.FindAsync(id);

            if (cityRoute == null)
            {
                return NotFound();
            }

            return cityRoute;
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
            catch (DbUpdateConcurrencyException)
            {
                if (!CityRouteExists(id))
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

        // POST: api/CityRoutes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CityRoute>> PostCityRoute(CityRoute cityRoute)
        {
            _context.CityRoutes.Add(cityRoute);
            await _context.SaveChangesAsync();

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
