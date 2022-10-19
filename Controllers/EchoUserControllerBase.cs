using echoStudy_webAPI.Areas.Identity.Data;
using echoStudy_webAPI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Controllers
{
    /// <summary>
    /// Implements an action filter which establishes the current user based on the JWT recieved
    /// in the authorization header
    /// </summary>
    public class EchoUserControllerBase : ControllerBase, IAsyncActionFilter
    {
        protected readonly UserManager<EchoUser> _um;
        protected EchoUser _user;

        public EchoUserControllerBase(UserManager<EchoUser> um)
        {
            _um = um;
        }

        // this is necessary to get endpoints.MapControllers() in Startup.cs not to throw an error
        // otherwise it thinks this function is a controller action that accepts two complex types
        // which is not possible
        [NonAction]
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // before action executes
            if(Request.Headers.ContainsKey("Authorization"))
            {
                var token = new JwtSecurityToken(Request.Headers["Authorization"].ToString().Split(' ')[1]);
                _user = await _um.FindByIdAsync(token.Subject);
            }
            else
            {
                _user = null;
            }

            // ---This executes the action---
            var result = await next();

            // after action executes

        }
    }
}
