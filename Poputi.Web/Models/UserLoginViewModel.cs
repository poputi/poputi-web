using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poputi.Web.Models
{
    public class UserLoginViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    
    public class UserRegistrationViewModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
    }
}
