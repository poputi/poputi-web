using Poputi.DataAccess.Daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Poputi.Logic.Interfaces
{
    public interface IRoutesService
    {
        ValueTask AddDriverRouteAsync(CityRoute cityRoute, CancellationToken cancellationToken = default);
        IAsyncEnumerable<CityRoute> FindRoutesWithinAsync(CityRoute cityRoute, double distance);
    }
}
