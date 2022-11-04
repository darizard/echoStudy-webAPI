using echoStudy_webAPI.Areas.Identity.Data;
using echoStudy_webAPI.Data.Requests;
using echoStudy_webAPI.Data.Responses;
using echoStudy_webAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : EchoUserControllerBase
    {
        private readonly EchoStudyDB _context;

        public UsersController(EchoStudyDB context, UserManager<EchoUser> um)
        : base(um)
        {
            _context = context;
        }

        // GET /users
        /// <summary>
        /// Retrieves all of the logged in user's information
        /// </summary>
        /// <remarks>
        /// User authentication is encoded in the JSON Web Token provided in the Authorization header
        /// </remarks>
        /// <response code="200">Returns the user's data</response>
        /// <response code="400">Token was not provided</response>
        /// <response code="500">Token was invalid</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserInfo()
        {
            // Ensure something is in the authorization header
            string[] authHeader = Request.Headers["Authorization"].ToString().Split(' ');
            if (authHeader.Length < 2)
            {
                return BadRequest("JSON Web Token in the authorization header required for this endpoint");
            }
            var token = new JwtSecurityToken(authHeader[1]);
            EchoUser user = await _um.FindByIdAsync(token.Subject);

            // Return their data
            return Ok(new UserInfoResponse
            {
                Email = user.Email,
                Username = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Id = user.Id,
                DateCreated = user.DateCreated
            });
        }

        // POST /users
        /// <summary>
        /// Updates an echo user
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="registerUserInfo">Credentials of the authenticating user as well as any info to be changed</param>
        /// <response code="200">Returns the ID of the updated user</response>
        /// <response code="400">User could not be updated with provided body OR no changes occured</response>
        /// <response code="401">Password is incorrect</response>
        /// <response code="404">User does not exist</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserUpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostEditUser([FromBody] RegisterUserRequest registerUserInfo)
        {
            // Password is always required
            if (registerUserInfo.Password is null)
            {
                return BadRequest("Password must be provided");
            }

            // Ennsure the user exists and the password matches
            EchoUser user;
            if (registerUserInfo.Email is not null)
            {
                user = await _um.FindByEmailAsync(registerUserInfo.Email);
            }
            else if (registerUserInfo.UserName is not null)
            {
                user = await _um.FindByNameAsync(registerUserInfo.UserName);
            }
            else
            {
                return BadRequest("Atleast username or email must be provided to identify the user");
            }
            if (user is null)
            {
                return NotFound();
            }
            if (!await _um.CheckPasswordAsync(user, registerUserInfo.Password))
            {
                return Unauthorized();
            }

            // Change user details that differ
            if (registerUserInfo.PhoneNumber is not null)
            {
                user.PhoneNumber = registerUserInfo.PhoneNumber;
            }
            if (registerUserInfo.UserName is not null)
            {
                user.UserName = registerUserInfo.UserName;
            }
            if (registerUserInfo.Email is not null && registerUserInfo.UserName is not null)
            {
                if (registerUserInfo.Email != user.Email)
                {
                    user.Email = registerUserInfo.Email;
                }
                if (registerUserInfo.UserName != user.UserName)
                {
                    user.UserName = registerUserInfo.UserName;
                }
            }

            // Try to save the updated user
            _context.Update(user);
            int result = await _context.SaveChangesAsync();

            if (result == 0)
            {
                return BadRequest("No changes were made");
            }
            else
            {
                return Ok(new UserUpdateResponse
                {
                    id = user.Id
                });
            }
        }

        // GET /users/names
        /// <summary>
        /// Retrieves a list of all usernames
        /// </summary>
        /// <response code="200">Returns a list of all usernames</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<string>), StatusCodes.Status200OK)]
        [HttpGet("names")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsernames()
        {
            // Compile a list of all usernames
            List<string> names = new List<string>();
            foreach (EchoUser user in await _um.Users.ToListAsync())
            {
                names.Add(user.UserName);
            }

            // Return the list
            return Ok(names);
        }
    }
}
