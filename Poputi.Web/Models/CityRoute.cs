using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.Web.Models
{
    /// <summary>
    /// Маршрут по городу.
    /// </summary>
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

        public double StartLongitude { get; set; }
        public double StartLatitude { get; set; }
        public double EndLongitude { get; set; }
        public double EndLatitude { get; set; }
    }
}
