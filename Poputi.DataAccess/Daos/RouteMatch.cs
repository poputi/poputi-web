using Poputi.DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.DataAccess.Daos
{
    public class RouteMatch
    {
        public Guid Id { get; set; }
        public RouteMatchType RouteMatchType { get; set; }
        public User Driver { get; set; }
        public ICollection<User> FellowTravelers { get; set; }
    }
}
