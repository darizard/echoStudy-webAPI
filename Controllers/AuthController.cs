using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using echoStudy_webAPI.Areas.Identity.Data;
using echoStudy_webAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using echoStudy_webAPI.Data;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

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

        // POST: /Authenticate
        // Retrieves a JSON Web Token related to a specific user
        // Required headers: 
        //      Content-Type: application/json
        //      Content-Length: <length>
        //      Host: <host>
        // Required body JSON:
        //      {
        //          "username": "<valid user>",
        //          "password": "<matching password>"
        //      }
        /// <summary>
        /// Generates and produces a JSON Web Token object for use in
        /// authentication and authorization for subsequent API calls.
        /// </summary>
        /// <remarks>If no parameter is specified, returns all deck objects.
        /// If userId or userEmail is specified, returns the decks related to the given user. If
        /// both parameters are specified, userId takes precedence.
        /// </remarks>
        /// <param name="userCreds">Credentials of the authenticating user (Subject)</param>
        /// <response code="200">Returns the JSON Web Token object</response>
        /// <response code="401">Invalids User Credentials were provided</response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        // I have no idea how to get this to show the actual fields of the object returned by
        // `return Unauthorized();`
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
