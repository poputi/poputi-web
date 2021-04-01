using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.DataAccess.Daos
{
    public class Driver : User
    {
        public List<Car> Cars { get; set; }
    }
}
