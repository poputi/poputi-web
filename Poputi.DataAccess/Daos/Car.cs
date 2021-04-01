using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.DataAccess.Daos
{
    public class Car
    {
        public int Id { get; set; }
        public int Capacity { get; set; }
        public string Name { get; set; }
        [ForeignKey("Driver")]
        public int DriverId { get; set; }
        public Driver Driver {get;set;}
    }
}
