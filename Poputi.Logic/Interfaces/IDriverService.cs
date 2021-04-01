using Poputi.DataAccess.Daos;
using System.Threading.Tasks;

namespace Poputi.Logic.Interfaces
{
    public interface IDriverService
    {
        Task AddCar(int driverId, Car car);
        
    }
}
