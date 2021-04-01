using Poputi.DataAccess.Contexts;
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
    public class RoutesService: IRoutesService
    {
        private readonly MainContext _mainContext;
        private readonly IPointOperationsService _pointOperationsService;

        public RoutesService(MainContext mainContext, IPointOperationsService pointOperationsService)
        {
            _mainContext = mainContext;
            _pointOperationsService = pointOperationsService;
        }

        public async ValueTask AddDriverRoute(User user, CityRoute cityRoute)
        {
            await _mainContext.CityRoutes.AddAsync(cityRoute).ConfigureAwait(false);
        }

        //public async ValueTask<IEnumerable<CityRoute>> SearchForFellowTraveler()
        //{
        //    //_pointOperationsService.GetByCloseRoutes()
        //}
    }
}
