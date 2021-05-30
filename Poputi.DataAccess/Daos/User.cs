using System.Collections.Generic;

namespace Poputi.DataAccess.Daos
{
    public class User
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public ICollection<CityRoute> HostedRoutes { get; set; }
        public City HomeCity { get; set; }
        public string PhoneNumber { get; set; }
    }
}
