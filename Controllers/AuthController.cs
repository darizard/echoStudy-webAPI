using Microsoft.AspNetCore.Mvc;
using echoStudy_webAPI.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using echoStudy_webAPI.Data;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using echoStudy_webAPI.Data.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace echoStudy_webAPI.Controllers
{
    //auth controller endpoints work from the base application URL
    [Route("")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static UserManager<EchoUser> _um;
        private readonly IJwtAuthenticationManager _jwtManager;
        
        public AuthController(UserManager<EchoUser> um,
                              IJwtAuthenticationManager jwtManager)
        {
            _um = um;
            _jwtManager = jwtManager;
        }

        /// <summary>
        /// Generates and produces a JSON Web Token object for use in
        /// authentication and authorization for subsequent API calls.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="userCreds">Credentials of the authenticating user (Subject)</param>
        /// <response code="200">Returns the JSON Web Token object</response>
        /// <response code="401">Invalids User Credentials were provided</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] UserCreds userCreds)
        {
            EchoUser user = await _um.FindByEmailAsync(userCreds.username.ToUpper());
            if (!await _um.CheckPasswordAsync(user, userCreds.password) 
                || user == null)
            {
                return Unauthorized();
            }

            var authResponse = await _jwtManager.AuthenticateUserAsync(user);
            return Ok(authResponse);
        }

        // GET: /Authenticate
        // If the supplied token is valid, returns the decoded JWT.
        // This endpoint currently exists for testing purposes.
        // Required headers:
        //      Host: <host>
        //      Authorization: Bearer <token>
        [HttpGet("authenticate")]
        public string Get()
        {

            var tokenAuthHeader = Request.Headers["Authorization"];
            string encodedToken = tokenAuthHeader.ToString().Split(' ')[1];
            var token = new JwtSecurityToken(encodedToken);

            return token.ToString();
        }

        /// <summary>
        /// Refreshes the provided JWT
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="request">Access and refresh token pair to be refreshed</param>
        /// <response code="200">Returns the JSON Web Token object</response>
        /// <response code="400">Invalid token</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var response = await _jwtManager.RefreshTokenAsync(request.Token, request.RefreshToken);

            if(response == null)
            {
                return BadRequest();
            }

            return Ok(response);
        }
    }
}
