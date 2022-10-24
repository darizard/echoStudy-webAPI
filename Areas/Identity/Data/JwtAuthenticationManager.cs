using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using echoStudy_webAPI.Areas.Identity.Data;
using System.Threading.Tasks;
using echoStudy_webAPI.Data.Responses;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using echoStudy_webAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace echoStudy_webAPI.Data
{
    public interface IJwtAuthenticationManager
    {
        Task<AuthenticationResponse> AuthenticateUserAsync(EchoUser user);
        Task<AuthenticationResponse> RefreshTokenAsync(string token, string refreshToken);
    }

    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly EchoStudyDB _context;
        private readonly UserManager<EchoUser> _um;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly JwtSettings _jwtSettings;

        public JwtAuthenticationManager(EchoStudyDB context,
                                        UserManager<EchoUser> um,
                                        TokenValidationParameters tokenValidationParameters,
                                        JwtSettings jwtSettings)
        {
            _context = context;
            _um = um;
            _tokenValidationParameters = tokenValidationParameters;
            _jwtSettings = jwtSettings;
        }

        public async Task<AuthenticationResponse> AuthenticateUserAsync(EchoUser user)
        {
            // make sure we can generate new tokens with no issue
            var response = await GenerateNewTokensAsync(user);
            if (response == null)
            {
                return response;
            }

            // if not anonymous auth, determine whether user has an active refresh token
            if (user != null)
            {
                var activeRefreshTokenQuery = from rt in _context.RefreshTokens
                                          where rt.UserId == user.Id &&
                                            rt.Used == false &&
                                            rt.Revoked == false
                                          select rt;

                // there should only be zero or one active tokens, but let's revoke them all just in case
                foreach (var token in await activeRefreshTokenQuery.ToListAsync())
                {
                    if (token.CreationDate < DateTime.UtcNow && token.ExpirationDate > DateTime.UtcNow)
                        token.Revoked = true;
                }
            }

            return response;
        }

        public async Task<AuthenticationResponse> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            // validate the token using a token handler and return the principal
            var validatedToken = GetPrincipalFromToken(accessToken);
            if(validatedToken == null)
            {
                return null;
            }

            // get expiration of the access token
            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(
                                            x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = DateTime.UnixEpoch.AddSeconds(expiryDateUnix);
            /*
            if(expiryDateTimeUtc > DateTime.UtcNow)
            {
                // the access token isn't expired yet
                return null;
            }*/

            // determine whether the refresh token received in the request exists in our database
            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync
                                                        (x => x.Token == refreshToken);
            if (storedRefreshToken == null)
            {
                // refresh token does not exist
                return null;
            }

            // determine whether the refresh token has expired
            if(DateTime.UtcNow > storedRefreshToken.ExpirationDate)
            {
                // refresh token is expired, cannot rotate, need a fresh authentication
                return null;
            }

            if(storedRefreshToken.Revoked)
            {
                // the refresh token has been revoked
                return null;
            }

            if(storedRefreshToken.Used)
            {
                // the refresh token has already been used
                return null;
            }

            // get the id of the access token
            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            if (storedRefreshToken.JwtId != jti)
            {
                // refresh token does not match the access token
                return null;
            }

            storedRefreshToken.Used = true;
            await _context.SaveChangesAsync();

            // ensure the user id encoded in the JWT exists in the database
            var user = await _um.FindByIdAsync(validatedToken.Claims.Single
                                        (x => x.Type == JwtRegisteredClaimNames.Sub).Value);
            
            return await GenerateNewTokensAsync(user);
        }

        private async Task<AuthenticationResponse> GenerateNewTokensAsync(EchoUser user)
        {
            string tokensub;
            string tokenemail;
            Claim[] roleClaims = null;
            if (user == null)
            {
                tokensub = "";
                tokenemail = "";
            }
            else
            {
                tokensub = user.Id.ToString();
                tokenemail = user.Email;
                var roleClaimsList = new List<Claim>();
                foreach(var r in await _um.GetRolesAsync(user))
                {
                    roleClaimsList.Add(new Claim(ClaimTypes.Role,r));
                }
                roleClaims = roleClaimsList.ToArray();
            }

            // need a security token handler
            var tokenHandler = new JwtSecurityTokenHandler();
            // need byte array of key
            var tokenKey = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            // per docs: This is a place holder for all the attributes related to the issued token.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // per docs: Gets or sets the output claims to be included in the issued token.
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, tokensub),
                    new Claim(JwtRegisteredClaimNames.Email, tokenemail),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())// we can add more to the JWT payload here
                }),
                // TEST VALUE. Change token expiry window for staging/prod
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            // Add role claims to the subject
            if(roleClaims is not null)
            {
                tokenDescriptor.Subject.AddClaims(roleClaims);
            }
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken
            {
                JwtId = accessToken.Id,
                UserId = tokensub,
                CreationDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddDays(14)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResponse
            {
                Token = tokenHandler.WriteToken(accessToken),
                RefreshToken = refreshToken.Token
            };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string accessToken, bool allowExpired = true)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var validationParams = _tokenValidationParameters;
                if (allowExpired)
                {
                    validationParams = validationParams.Clone();
                    validationParams.ValidateLifetime = false;  // ignore token expiration
                }

                var principal = tokenHandler.ValidateToken
                    (accessToken, validationParams, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals("HS256");
        }
    }
}
