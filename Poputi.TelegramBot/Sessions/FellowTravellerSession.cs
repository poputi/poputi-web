using NetTopologySuite.Geometries;
using Poputi.DataAccess.Daos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.TelegramBot.Sessions
{
    public class FellowTravellerSession
    {
        public Point Start { get; set; }
        public Point End { get; set; }
        public string DateTime { get; set; }
        public Dictionary<string, CityRoute> RoutesOptions { get; } = new Dictionary<string, CityRoute>();
    }
}
