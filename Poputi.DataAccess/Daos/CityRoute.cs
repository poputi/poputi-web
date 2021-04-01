using NetTopologySuite.Geometries;
using Poputi.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.DataAccess.Daos
{
    public class CityRoute
    {
        public int Id { get; set; }

        /// <summary>
        /// Создатель маршрута.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Создатель маршрута.
        /// </summary>
        public User User { get; set; }

        public Point Start { get; set; }
        public Point End { get; set; }
        public CityRouteType CityRouteType { get; set; }
    }
}
