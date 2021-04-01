using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.DataAccess.Daos
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // TODO: я хз что это, но можно проверить
        //[Column(TypeName = "geography (point)")]
        public Point Location { get; set; }
    }
}
