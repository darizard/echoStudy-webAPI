using echoStudy_webAPI.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static echoStudy_webAPI.Controllers.DecksController;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using echoStudy_webAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using echoStudy_webAPI.Data;
using Amazon.S3.Model;
using static echoStudy_webAPI.Data.DbInitializer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using echoStudy_webAPI.Data.Responses;
using System.Security.Cryptography;
using System.Text;

namespace echoStudy_webAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PublicController : EchoUserControllerBase
    {
        EchoStudyDB _context;

        public PublicController(UserManager<EchoUser> um,
                                EchoStudyDB context) : base(um)
        {
            _context = context;
        }

        /**
        * Define response type for copying a deck
        */
        public class DeckCopyResponse
        {
            public int id { get; set; }
            public DateTime dateCreated { get; set; }
        }


        // GET: /Public/Decks
        /// <summary>
        /// Retrieves all Public Decks
        /// </summary>
        /// <remarks>
        /// All Decks with an access level of Public
        /// </remarks>
        /// <response code="200">Returns all Public Deck objects not owned by the currently authenticated user</response>
        [HttpGet("decks")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<DeckInfo>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DeckInfo>>> GetPublicDecks()
        {
            string userId = _user?.Id;

            // Query the DB for the deck objects
            var query = from d in _context.Decks.Include(d => d.Cards)
                                                .Include(d => d.DeckOwner)
                                                .Include(d => d.OrigAuthor)
                        where d.Access == Access.Public &&
                              d.UserId != userId
                        select d;
            var decks = await query.ToListAsync();

            // Build the deck info objects
            List<DeckInfo> deckInfo = new List<DeckInfo>();
            foreach (Deck d in decks)
            {
                deckInfo.Add(new DeckInfo
                {
                    id = d.DeckID,
                    title = d.Title,
                    description = d.Description,
                    access = d.Access.ToString(),
                    default_flang = d.DefaultFrontLang.ToString(),
                    default_blang = d.DefaultBackLang.ToString(),
                    cards = d.Cards.Select(c => c.CardID).ToList(),
                    ownerId = d.UserId,
                    ownerUserName = d.DeckOwner.UserName,
                    studiedPercent = (double) d.StudyPercent,
                    masteredPercent = (double) d.MasteredPercent,
                    date_created = d.DateCreated,
                    date_touched = d.DateTouched,
                    date_updated = d.DateUpdated,
                    orig_deck_id = d.OrigDeckId,
                    orig_author_id = d.OrigAuthorId,
                    orig_author_name = d.OrigAuthorId is null || d.OrigAuthorId == "" ? null : d.OrigAuthor.UserName
                });
            }

            return Ok(deckInfo);
        }

        // POST: /Public/copy/deck={id}
        /// <summary>
        /// Copies another user's deck if it's public to the logged in user
        /// </summary>
        /// <remarks>
        /// User authentication is encoded in the JSON Web Token provided in the Authorization header
        /// </remarks>
        /// <param name="id">ID of the Deck to copy</param>
        /// <response code="201">Returns the Id and DateUpdated of the copied Deck</response>
        /// <response code="400">Owner tried to copy their own deck</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The deck is private and therefore not available for copying</response>
        /// <response code="404">Object at the deck id provided was not found</response>
        /// <response code="500">Database failed to save despite valid request</response>
        [HttpPost("copy/deck={id}")]
        [Produces("application/json")]
        [Authorize]
        [ProducesResponseType(typeof(DeckCreationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ShareDeck(int id)
        {
            if(_user is null)
            {
                return Unauthorized(new { error = "You are not logged in" });
            }

            // Ensure it exists
            Deck deck = await _context.Decks.FindAsync(id);
            if (deck is null)
            {
                return NotFound(new { error = $"Deck with id {id} does not exist" });
            }
            // Ensure it's public
            if(deck.Access != Access.Public)
            {
                return Forbid();
            }
            // Ensure the owner isn't copying their own deck
            if(deck.UserId == _user.Id)
            {
                return BadRequest(new { error = "This deck already belongs to you!" });
            }

            // Copy the deck
            Deck copyDeck = new Deck
            {
                DeckCategories = new List<DeckCategory>(),
                Cards = new List<Card>(),
                Title = deck.Title,
                Description = deck.Description,
                Access = deck.Access,
                DefaultFrontLang = deck.DefaultFrontLang,
                DefaultBackLang = deck.DefaultBackLang,
                UserId = _user.Id,
                DateCreated = DateTime.Now,
                DateTouched = DateTime.Now,
                DateUpdated = DateTime.Now,
                OrigDeckId = deck.OrigDeckId is null ? deck.DeckID : deck.OrigDeckId,
                OrigAuthorId = deck.OrigAuthorId is null || deck.OrigAuthorId == "" ? deck.UserId : deck.OrigAuthorId
            };

            // Grab and copy all the cards
            var cardQuery = from c in _context.Cards
                            where c.DeckID == id
                            select c;
            List<Card> cards = await cardQuery.ToListAsync();
            foreach (Card c in cards)
            {
                Card copyCard = new Card
                {
                    FrontText = c.FrontText,
                    BackText = c.BackText,
                    FrontAudio = c.FrontAudio,
                    BackAudio = c.BackAudio,
                    FrontLang = c.FrontLang,
                    BackLang = c.BackLang,
                    UserId = _user.Id,
                    Deck = copyDeck,
                    DeckPosition = c.DeckPosition,
                    DateCreated = DateTime.Now,
                    DateTouched = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    Score = 0
                };
                copyDeck.Cards.Add(copyCard);
            }

            // Try to save the copies to the database
            await _context.Decks.AddAsync(copyDeck);
            foreach (Card c in copyDeck.Cards)
            {
                await _context.Cards.AddAsync(c);
            }

            // Try to save
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(500, e.Message);
            }

            // Indicate success
            return CreatedAtAction("ShareDeck", new DeckCopyResponse
            {
                id = copyDeck.DeckID,
                dateCreated = deck.DateCreated
            });
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
                ProfilePicture = "https://gravatar.com/avatar/" + MD5.HashData(Encoding.ASCII.GetBytes(user.Email.Trim().ToLower())) + "?d=retro",
                Decks = await query.ToListAsync()
            });
        }

        /// <summary>
        /// Retrieves an echo user's public details through provided username or email
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <response code="200">Returns nonsensitive, public details of every user</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<UserInfoPublicResponse>), StatusCodes.Status200OK)]
        [HttpGet("users")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllUserPublicInfo()
        {
            // The users
            var userQuery = from u in _context.Users
                            select u;
            var users = await userQuery.ToListAsync();

            // Build a UserPublicInfoResponse object for every user
            List<UserInfoPublicResponse> userInfo = new List<UserInfoPublicResponse>();
            foreach(EchoUser user in users)
            {
                // Things that can immediately be assigned
                UserInfoPublicResponse publicInfo = new UserInfoPublicResponse
                {
                    Username = user.UserName,
                    ProfilePicture = "https://gravatar.com/avatar/" + MD5.HashData(Encoding.ASCII.GetBytes(user.Email.Trim().ToLower())) + "?d=retro"
                };

                // Decks must be pulled from the database
                var deckQuery = from d in _context.Decks
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
                publicInfo.Decks = await deckQuery.ToListAsync();

                // Finally add their info to the list
                userInfo.Add(publicInfo);
            }

            // Return their details
            return Ok(userInfo);
        }
    }
}
