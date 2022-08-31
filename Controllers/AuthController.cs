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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace echoStudy_webAPI.Controllers
{
    //auth controller endpoints work from the base application URL
    [Route("")]
    [ApiController]
    public class AuthController : EchoControllerBase
    {
        private readonly EchoStudyDB _context_EchoStudyDB;
        private readonly IConfiguration _configuration;
        private static UserManager<EchoUser> _um;
        
        public AuthController(EchoStudyDB echoStudyDB, IConfiguration configuration,
                              UserManager<EchoUser> um,
                              IJwtAuthenticationManager jwtManager) : base(jwtManager)
        {
            _context_EchoStudyDB = echoStudyDB;
            _configuration = configuration;
            _um = um;
            
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
        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] UserCreds userCreds)
        {
            EchoUser user = await _um.FindByEmailAsync(userCreds.username.ToUpper());
            if (!await _um.CheckPasswordAsync(user, userCreds.password) 
                || user == null)
            {
                return NotFound();
            }

            var token = _jwtManager.Authenticate(user.Email);
            return Ok(token);
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

        /*
        // GET api/<AuthController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        */
    }
}
