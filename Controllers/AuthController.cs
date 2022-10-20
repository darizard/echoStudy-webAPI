using Microsoft.AspNetCore.Mvc;
using echoStudy_webAPI.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using echoStudy_webAPI.Data;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using echoStudy_webAPI.Data.Responses;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using echoStudy_webAPI.Models;
using System.Linq;
using System.Collections.Generic;
using echoStudy_webAPI.Data.Requests;
using static echoStudy_webAPI.Controllers.DecksController;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace echoStudy_webAPI.Controllers
{
    /**
     * Controller responsible for dealing with anything related to Authorization or Identity
     */
    [Route("")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static UserManager<EchoUser> _um;
        private readonly IJwtAuthenticationManager _jwtManager;
        private readonly EchoStudyDB _context;

        /**
         * Initializes AuthController
         */
        public AuthController(UserManager<EchoUser> um,
                              IJwtAuthenticationManager jwtManager,
                              EchoStudyDB context)
        {
            _um = um;
            _jwtManager = jwtManager;
            _context = context;
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
            if (userCreds.username.IsNullOrEmpty() && userCreds.password.IsNullOrEmpty())
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

            if (response == null)
            {
                return BadRequest();
            }

            return Ok(response);
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
        public async Task<IActionResult> PostRegister([FromBody] RegisterUserRequest registerUserInfo)
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
        /// Deletes an EchoUser and all of their content
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="registerUserInfo">Credentials of the authenticating user (Subject)</param>
        /// <response code="204">Returns no content if the user was deleted successfully</response>
        /// <response code="400">User deletion could not be completed with the given request body OR identity failed deletion</response>
        /// <response code="401">Password is incorrect</response>
        /// <response code="404">User does not exist</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IdentityError[]), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [HttpPost("deregister")]
        [AllowAnonymous]
        public async Task<IActionResult> PostDeregister([FromBody] RegisterUserRequest registerUserInfo)
        {
            // Null checks
            if(registerUserInfo.Email is null)
            {
                return BadRequest("Email must be provided");
            }
            if (registerUserInfo.Password is null)
            {
                return BadRequest("Password must be provided");
            }
            if (registerUserInfo.PhoneNumber is null)
            {
                return BadRequest("Phone number must be provided");
            }
            if (registerUserInfo.UserName is null)
            {
                return BadRequest("Username must be provided");
            }

            // Ennsure the user exists and everything is right
            EchoUser user = await _um.FindByEmailAsync(registerUserInfo.Email);
            if (user is null)
            {
                return NotFound();
            }
            if (!await _um.CheckPasswordAsync(user, registerUserInfo.Password))
            {
                return Unauthorized();
            }
            if(registerUserInfo.PhoneNumber != user.PhoneNumber)
            {
                return BadRequest("Provided phone number does not match");
            }
            if (user.UserName.ToLower() != registerUserInfo.UserName.ToLower())
            {
                return BadRequest("Provided username does not match");
            }

            // Try to delete the user
            var identityResult = await _um.DeleteAsync(user);
            if (identityResult.Succeeded)
            {
                // User was deleted, now delete all of their data

                // Categories
                var categoryQuery = from dc in _context.DeckCategories
                            where dc.UserId == user.Id
                            select dc;
                List<DeckCategory> deckCategories = await categoryQuery.ToListAsync();
                foreach (DeckCategory deckCategory in deckCategories)
                {
                    _context.DeckCategories.Remove(deckCategory);
                }

                // Decks
                var deckQuery = from d in _context.Decks
                        where d.UserId == user.Id
                        select d;
                List<Deck> decks = await deckQuery.ToListAsync();
                foreach (Deck deck in decks)
                {
                    _context.Decks.Remove(deck);
                }

                // Cards
                var cardQuery = from c in _context.Cards
                                where c.UserId == user.Id
                                select c;
                List<Card> cards = await cardQuery.ToListAsync();
                foreach (Card card in cards)
                {
                    _context.Cards.Remove(card);
                }

                // Tokens
                var tokenQuery = from t in _context.RefreshTokens
                                where t.UserId == user.Id
                                select t;
                List<RefreshToken> tokens = await tokenQuery.ToListAsync();
                foreach (RefreshToken refreshToken in tokens)
                {
                    _context.RefreshTokens.Remove(refreshToken);
                }

                // Save
                await _context.SaveChangesAsync();

                return NoContent();
            }
            else
            {
                return BadRequest(identityResult.Errors);
            }
        }

        /// <summary>
        /// Changes password for given echoUser
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="changePasswordInfo"> Important credentials of the authenticating user as well as their new password (Subject)</param>
        /// <response code="200">Returns the ID of the updated user</response>
        /// <response code="400">The new password did not meet security requirements OR the action could not be completed with provided request body</response>
        /// <response code="401">Password is incorrect</response>
        /// <response code="404">User does not exist</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserUpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IdentityError[]), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [HttpPost("changepassword")]
        [AllowAnonymous]
        public async Task<IActionResult> PostChangePassword([FromBody] ChangePasswordRequest changePasswordInfo)
        {
            // Null/logic checks
            if (changePasswordInfo.Email is null)
            {
                return BadRequest("Email must be provided");
            }
            if (changePasswordInfo.PhoneNumber is null)
            {
                return BadRequest("Phone number must be provided");
            }
            if (changePasswordInfo.OldPassword is null)
            {
                return BadRequest("Old password must be provided");
            }
            if (changePasswordInfo.NewPassword is null)
            {
                return BadRequest("New password must be provided");
            }
            if (changePasswordInfo.NewPassword == changePasswordInfo.OldPassword)
            {
                return BadRequest("New and old password must be different");
            }

            // Ennsure the user exists and everything is right (Phone number and old password match)
            EchoUser user = await _um.FindByEmailAsync(changePasswordInfo.Email);
            if (user is null)
            {
                return NotFound();
            }
            if (changePasswordInfo.PhoneNumber != user.PhoneNumber)
            {
                return BadRequest("Provided phone number does not match");
            }
            if (!await _um.CheckPasswordAsync(user, changePasswordInfo.OldPassword))
            {
                return Unauthorized();
            }

            // Change the password using a password token and try to save
            var passwordToken = await _um.GeneratePasswordResetTokenAsync(user);
            var identityResult = await _um.ResetPasswordAsync(user, passwordToken, changePasswordInfo.NewPassword);
            if (identityResult.Succeeded)
            {
                // Save
                await _context.SaveChangesAsync();

                return Ok(new UserUpdateResponse
                {
                    id = user.Id
                });
            }
            else
            {
                return BadRequest(identityResult.Errors);
            }
        }

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
        [HttpGet("users")]
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
                Id = user.Id
            });
        }

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
        [HttpPost("users")]
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

        /// <summary>
        /// Retrieves an echo user's public details through provided username or email
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="username">Username OR email of the target user</param>
        /// <response code="200">Returns nonsensitive, public details of a user</response>
        /// <response code="400">Username was not provided</response>
        /// <response code="404">User does not exist</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(UserInfoPublicResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [HttpGet("users/{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUserPublicInfo(string username)
        {
            // Grab the user
            EchoUser user;
            if (username is not null)
            {
                if (username.Contains('@'))
                {
                    user = await _um.FindByEmailAsync(username);
                }
                else
                {
                    user = await _um.FindByNameAsync(username);
                }
            }
            else
            {
                return BadRequest("Username is required to find the user");
            }

            // User not found
            if (user is null)
            {
                return NotFound();
            }

            // Get their public decks
            var query = from d in _context.Decks
                        where d.UserId == user.Id && d.Access == Access.Public
                        select new DeckInfo
                        {
                            id = d.DeckID,
                            title = d.Title,
                            description = d.Description,
                            access = d.Access.ToString(),
                            default_flang = d.DefaultFrontLang.ToString(),
                            default_blang = d.DefaultBackLang.ToString(),
                            cards = d.Cards.Select(c => c.CardID).ToList(),
                            ownerId = d.UserId,
                            date_created = d.DateCreated,
                            date_touched = d.DateTouched,
                            date_updated = d.DateUpdated
                        };

            // Return their details
            return Ok(new UserInfoPublicResponse
            {
                Username = user.UserName,
                Decks = await query.ToListAsync(),
                ProfilePicture = "https://gravatar.com/avatar/" + MD5.HashData(Encoding.ASCII.GetBytes(user.Email.Trim().ToLower())) + "?d=retro"
            });
        }

        /// <summary>
        /// Retrieves a list of all usernames
        /// </summary>
        /// <response code="200">Returns a list of all usernames</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<string>), StatusCodes.Status200OK)]
        [HttpGet("users/names")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsernames()
        {
            // Compile a list of all usernames
            List<string> names = new List<string>();
            foreach (EchoUser user in _um.Users.ToList())
            {
                names.Add(user.UserName);
            }

            // Return the list
            return Ok(names);
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
    }
}
