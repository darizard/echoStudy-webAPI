using Amazon.Auth.AccessControlPolicy;
using echoStudy_webAPI.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : EchoUserControllerBase
    {
        public AdminController(UserManager<EchoUser> um) : base(um)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Endpoint()
        {
            if(_user is null || !await _um.IsInRoleAsync(_user, "Administrator"))
            {
                return Forbid();
            }

            return Ok("Ok");
        }
    }
}
