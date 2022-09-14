using Microsoft.AspNetCore.Mvc;
using echoStudy_webAPI.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using echoStudy_webAPI.Data;
using echoStudy_webAPI.Data.Requests;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using echoStudy_webAPI.Data.Responses;
using System;
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

        /// <summary>
        /// Creates an EchoUser
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="registerUserInfo">Credentials of the authenticating user (Subject)</param>
        /// <response code="200">Returns the ID of the newly created user</response>
        /// <response code="400">User registration could not be completed with the given request body</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(RegisterUserSuccess), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IdentityError[]), StatusCodes.Status400BadRequest)]
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerUserInfo)
        {
            var user = new EchoUser
            {
                UserName = registerUserInfo.UserName,
                Email = registerUserInfo.Email,
                PhoneNumber = registerUserInfo.PhoneNumber
            };
            var identityResult = await _um.CreateAsync(user, registerUserInfo.Password);
            if(identityResult.Succeeded)
            {
                return Ok(new RegisterUserSuccess
                {
                    Id = user.Id
                });
            }
            else
            {
                return BadRequest(identityResult.Errors);
            }


            /*
            EchoUser user = await _um.FindByEmailAsync(userCreds.username.ToUpper());
            if (!await _um.CheckPasswordAsync(user, userCreds.password)
                || user == null)
            {
                return Unauthorized();
            }

            var authResponse = await _jwtManager.AuthenticateUserAsync(user);
            return Ok(authResponse);*/
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
        public async Task<IActionResult> Authenticate([FromBody] UserCreds userCreds = null)
        {
            AuthenticationResponse authResponse;
            Console.WriteLine("break here");
            if(userCreds.username.IsNullOrEmpty() && userCreds.password.IsNullOrEmpty())
            {
                authResponse = await _jwtManager.AuthenticateUserAsync(null);
                return Ok(authResponse);
            }
            EchoUser user = await _um.FindByEmailAsync(userCreds.username.ToUpper());
            if (!await _um.CheckPasswordAsync(user, userCreds.password) 
                || user == null)
            {
                return Unauthorized();
            }

            authResponse = await _jwtManager.AuthenticateUserAsync(user);
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
