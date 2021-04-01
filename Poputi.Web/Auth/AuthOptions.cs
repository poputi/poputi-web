using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Poputi.Web.Auth
{
    public class AuthOptions
    {
        public const string ISSUER = "server";
        public const string AUDIENCE = "client";
        const string KEY = "lkkjasdfklabd!sf";
        public const int LIFETIME = 60;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
