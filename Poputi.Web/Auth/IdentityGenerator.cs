using Poputi.Logic.Interfaces;
using Poputi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Poputi.Web.Auth
{
    public class IdentityGenerator
    {
        private readonly IUserService _userService;

        public IdentityGenerator(IUserService userService)
        {
            _userService = userService;    
        }
        /// <summary>
        /// Вернет null если пользователь не авторизован
        /// </summary>
        /// <param name="userLoginViewModel"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GetIdentity(UserLoginViewModel userLoginViewModel)
        {
            ClaimsIdentity identity = null;
            var user = await _userService.Get(userLoginViewModel.Login);
            if (user != null)
            {
                var sha265 = new SHA256Managed();
                var passwordHash = Convert.ToBase64String(sha265.ComputeHash(Encoding.UTF8.GetBytes(userLoginViewModel.Password)));
                if (passwordHash == user.Password)
                {
                    var claims = new List<Claim>
                    {
                        new Claim("login", user.Login),
                        new Claim("auId", user.Id.ToString())
                    };
                    identity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
                }
            }
            return identity;
        }
    }
}
