using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Poputi.Web.Auth;
using Poputi.Web.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Poputi.Web.Controllers
{
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IdentityGenerator _identityGenerator;
        public IdentityController(IdentityGenerator identityGenerator)
        {
            _identityGenerator = identityGenerator;
        }

        [HttpGet]
        public async Task<IActionResult> Token(UserLoginViewModel user)
        {
            var identity = await _identityGenerator.GetIdentity(user);
            if (identity == null)
            {
                return Unauthorized();
            }

            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new JsonResult(encodedJwt);
        }
    }
}
