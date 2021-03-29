using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poputi.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poputi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly IDriverService _driverService;
        public CarController(IDriverService driverService)
        {
            _driverService = driverService;
        }


        [HttpGet]
        public async Task<IActionResult> GetCars()
        {
            throw new NotImplementedException();
        }
    }
}
