using Poputi.DataAccess.Contexts;
using Poputi.DataAccess.Daos;
using Poputi.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.Logic.Services
{
    public class PointOperationsService : IPointOperationsService
    {
        private readonly MainContext _mainContext;

        public PointOperationsService(MainContext mainContext)
        {
            _mainContext = mainContext;
        }

        public IEnumerable<User> GetByCloseRoutes(User user, double distance)
        {
            if (user.HostedRoutes.Count == 0)
            {
                return Array.Empty<User>();
            }
            return _mainContext.Users.AsQueryable().Where(x => x.HostedRoutes.Count > 0 &&
                x.HostedRoutes.First().Start.IsWithinDistance(user.HostedRoutes.First().Start, distance) &&
                x.HostedRoutes.First().End.IsWithinDistance(user.HostedRoutes.First().End, distance));
        }
    }
}
