using echoStudy_webAPI.Areas.Identity.Data;
using echoStudy_webAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Controllers
{
    public class UserActionFilter : ControllerBase, IAsyncActionFilter
    {
        protected readonly IJwtAuthenticationManager _jwtManager;
        protected readonly EchoUser _user;

        public UserActionFilter(IJwtAuthenticationManager jwtManager)
        {
            _jwtManager = jwtManager;
        }

        // [NonAction] is necessary to get endpoints.MapControllers() in Startup.cs not to throw an error
        // otherwise it thinks this function is a controller action that accepts two complex types
        // which is not possible
        [NonAction]
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // before action executes
            Console.WriteLine("before action");

            // ---This executes the action---
            var result = await next();
            Console.WriteLine("after action");

            //throw new System.NotImplementedException();
        }
    }
}
