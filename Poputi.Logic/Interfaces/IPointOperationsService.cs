using Poputi.DataAccess.Daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.Logic.Interfaces
{
    public interface IPointOperationsService
    {
        IEnumerable<User> GetByCloseRoutes(User user, double distance);
    }
}
