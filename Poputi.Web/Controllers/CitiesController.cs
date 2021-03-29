using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Poputi.DataAccess.Daos;
using Poputi.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Poputi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly IGenericRepository<City> _repository;

        public CitiesController(IGenericRepository<City> repository)
        {
            _repository = repository;
        }

        // GET: api/Cities
        [HttpGet]
        public async ValueTask<ActionResult<List<City>>> GetCities(CancellationToken cancellationToken)
        {
            return await _repository.Read().ToListAsync(cancellationToken);
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async ValueTask<ActionResult<City>> GetCity(int id, CancellationToken cancellationToken)
        {
            City city = await _repository.Read(new object[] { id }, token: cancellationToken);
            if (city is null)
            {
                return NotFound();
            }
            return city;
        }

        //// PUT: api/Cities/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, City city, CancellationToken cancellationToken)
        {
            if (id != city.Id)
            {
                return BadRequest();
            }

            _repository.Update(city);

            try
            {
                await _repository.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CityExists(id))
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

        //// POST: api/CityRoutes
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async ValueTask<ActionResult<City>> PostCity(City city, CancellationToken cancellationToken)
        {
            await _repository.Create(city, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return CreatedAtAction("GetCity", new { id = city.Id }, city);
        }

        //// DELETE: api/CityRoutes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id, CancellationToken cancellationToken)
        {
            var city = await _repository.Read(new object[] { id }, cancellationToken);
            if (city is null)
            {
                return NotFound();
            }

            _repository.Remove(city);
            await _repository.SaveChangesAsync(cancellationToken);

            return NoContent();
        }

        private ValueTask<bool> CityExists(int id)
        {
            return _repository.ExistsAsync(x => x.Id == id);
        }
    }
}
