using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Poputi.DataAccess.Daos;

namespace Poputi.Logic.Interfaces
{
    public interface IDriverService
    {
        Task AddCar(int driverId, Car car);
        
    }
}
