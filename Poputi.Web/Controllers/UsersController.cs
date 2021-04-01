using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Daos;
using Poputi.Logic.Interfaces;
using Poputi.Web.Models;

namespace Poputi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MainContext _context;
        private readonly IDriverService _driverService;

        public UsersController(MainContext context, IDriverService driverService)
        {
            _context = context;
            _driverService = driverService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.AsQueryable().ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostUser(UserRegistrationViewModel userViewModel)
        {
            var user = new User();
            user.LastName = userViewModel.LastName;
            user.FirstMidName = userViewModel.FirstMidName;
            user.Login = userViewModel.Login;

            var sha256 = new SHA256Managed();
            user.Password = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(userViewModel.Password)));

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user.Id);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet]
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        
        [HttpPost]
        [Route("[action]")]
        [Authorize]
        public async Task<IActionResult> AddCar(CarViewModel carViewModel)
        {
            var userId = User.Claims.ToList().First(c => c.Type == "auId").Value;
            //TODO вынести в одно место и создать сервис для работы с клаймсами    
            var car = new Car();
            car.Name = carViewModel.Name;
            car.Capacity = carViewModel.Capacity;
            await _driverService.AddCar(int.Parse(userId), car);
            return Ok();
        }
    }
}
