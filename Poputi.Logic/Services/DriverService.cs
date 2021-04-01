using Poputi.DataAccess.Daos;
using Poputi.DataAccess.Interfaces;
using Poputi.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.Logic.Services
{
    public class DriverService : IDriverService
    {
        private readonly IGenericRepository<Car> _carRepository;
        private readonly IGenericRepository<Driver> _driverRepostory;
        public DriverService(IGenericRepository<Car> carRepository, 
             IGenericRepository<Driver> driverRepository)
        {
            _carRepository = carRepository;
            _driverRepostory = driverRepository;
        }
        public async Task AddCar(int driverId, Car car)
        {
            car.DriverId = driverId;
            await _carRepository.Create(car);
            await _carRepository.SaveChangesAsync();
        }
    }
}
