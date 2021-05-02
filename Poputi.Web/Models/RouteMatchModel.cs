using Poputi.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.Web.Models
{
    public class RouteMatchModel
    {
        public Guid Id { get; set; }
        public RouteMatchType RouteMatchType { get; set; }
        public ICollection<int> FellowTravelers { get; set; }
        /// <summary>
        /// Маршрут на двоих.
        /// </summary>
        public int MatchedCityRoute { get; set; }
    }
}
