using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using echoStudy_webAPI.Models;
using echoStudy_webAPI.Data;
using echoStudy_webAPI.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace echoStudy_webAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DecksController : EchoUserControllerBase
    {
        private readonly EchoStudyDB _context;

        public DecksController(EchoStudyDB context, UserManager<EchoUser> um)
            : base(um)
        {
            _context = context;
        }

        /**
         * This class should contain all information needed in a GET request
         */
        public class DeckInfo
        {
            public int id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string access { get; set; }
            public string default_flang { get; set; }
            public string default_blang { get; set; }
            public string ownerId { get; set; }
            public List<int> cards { get; set; }
            public DateTime date_created { get; set; }
            public DateTime date_updated { get; set; }
            public DateTime date_touched { get; set; }
        }

        /**
         * This class should contain all information needed from the user to create a row in the database
         */
        public class PostDeckInfo
        {
            public string title { get; set; }
            public string description { get; set; }
            public string access { get; set; }
            public string default_flang { get; set; }
            public string default_blang { get; set; }
            public string userEmail { get; set; }
            public string userId { get; set; }
            public List<int> cardIds { get; set; }
        }

        /**
         * Define response type for Deck creation
         */
        public class DeckCreationResponse
        {
            public int id { get; set; }
            public DateTime dateCreated { get; set; }
        }

        /**
         * Define response type for Deck update
         */
        public class DeckUpdateResponse
        {
            public int id { get; set; }
            public DateTime dateUpdated { get; set; }
        }

        // GET: /Decks
        /// <summary>
        /// Retrieves all Deck objects belonging to the authenticated user
        /// </summary>
        /// <remarks>
        /// User authentication is encoded in the JSON Web Token provided in the Authorization header
        /// </remarks>
        /// <response code="200">Returns the user's Deck objects</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<DeckInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<DeckInfo>>> GetDecks()
        {
            var query = from d in _context.Decks
                        where d.UserId == _user.Id
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
            return Ok(await query.ToListAsync());
        }

        // ******TODO: Does not depend at all on current user. Move to a "Public" controller?
        // GET: /Decks/Public
        /// <summary>
        /// Retrieves Public Decks
        /// </summary>
        /// <remarks>
        /// All Decks with an access level of Public
        /// </remarks>
        /// <response code="200">Returns all Public Deck objects</response>
        [HttpGet("Public")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<DeckInfo>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DeckInfo>>> GetPublicDecks()
        {
            var query = from d in _context.Decks
                        where d.Access == Data.Access.Public
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
            return Ok(await query.ToListAsync());
        }

        // GET: /Decks/{id}
        /// <summary>
        /// Retrieves one Deck specified by Id. Can be used to retrieve a user's own deck or a Public deck.
        /// NOTE: ALWAYS RETURNS THE ID OF THE OWNER. THIS ID WILL SHOW WHETHER THE AUTHORIZED USER IS THE
        /// OWNER OR NOT
        /// </summary>
        /// <remarks>
        /// <param name="id">Deck ID</param>
        /// </remarks>
        /// <response code="200">Returns the queried Deck object</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to access this deck</response>
        /// <response code="404">Deck object with the given id was not found</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Deck), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeckInfo>> GetDeck(int id)
        {
            // d.DeckID is unique; queryDeck returns only one deck

            // first query to see if the desired deck exists
            var queryDeck = from d in _context.Decks
                            where d.DeckID == id
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

            if (!await queryDeck.AnyAsync())
            {
                return NotFound();
            }

            // then query to see if the deck is accessible to that user
            // NOTE: ToListAsync() is necessary so we can leverage ToString() in queryDeck above
            var queryAuth = from d in await queryDeck.ToListAsync() 
                            where d.ownerId == _user.Id || d.access == Access.Public.ToString()
                            select new DeckInfo
                            {
                                id = d.id,
                                title = d.title,
                                description = d.description,
                                access = d.access,
                                default_flang = d.default_flang,
                                default_blang = d.default_blang,
                                cards = d.cards,
                                ownerId = d.ownerId,
                                date_created = d.date_created,
                                date_touched = d.date_touched,
                                date_updated = d.date_updated
                            };

            // NOTE: queryAuth is IEnumerable and not IQueryable now
            // no async methods necessary or available beyond this point
            if (!queryAuth.Any()) 
            {
                return Forbid();
            }

            return Ok(queryAuth.First());
        }

        // POST: /Decks/{id}
        /// <summary>
        /// Edits an existing Deck owned by the authenticated user
        /// </summary>
        /// <remarks>
        /// Currently does not support changing its associated cards
        /// </remarks>
        /// <param name="id">ID of the Deck to edit</param>
        /// <param name="deckInfo">
        /// Optional: title, description, default_flang, default_blang, userId, access, cardIds
        /// </param>
        /// <response code="200">Returns the Id and DateUpdated of the edited Deck</response>
        /// <response code="400">Invalid input or input type</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to edit this deck</response>
        /// <response code="404">Object at the deckId provided was not found</response>
        [HttpPost("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DeckUpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostDeckEdit(int id, PostDeckInfo deckInfo)
        {
            Deck deck;
            var deckQuery = from d in _context.Decks
                            where d.DeckID == id
                            select d;
            if((deck = await deckQuery.FirstAsync()) is null) return NotFound("Deck id " + id + " not found");


            var authQuery = from d in deckQuery
                            where d.UserId == _user.Id
                            select d;
            if (!await authQuery.AnyAsync()) return Forbid();

            if (deckInfo.title is not null)
            { 
                if (deckInfo.title == "") return BadRequest("title cannot be empty");
                deck.Title = deckInfo.title;
            }

            if (deckInfo.description is not null)
            {
                if (deckInfo.description == "") return BadRequest("description cannot be empty");
                deck.Description = deckInfo.description;
            }

            if (deckInfo.access is not null)
            {
                switch (deckInfo.access.ToLower())
                {
                    case "public":
                        deck.Access = Access.Public;
                        break;
                    case "private":
                        deck.Access = Access.Private;
                        break;
                    default:
                        return BadRequest("Valid access parameters are Public and Private");
                }
            }
            
            if (deckInfo.default_flang is not null)
            {
                switch (deckInfo.default_flang.ToLower())
                {
                    case "english":
                        deck.DefaultFrontLang = Language.English;
                        break;
                    case "spanish":
                        deck.DefaultFrontLang = Language.Spanish;
                        break;
                    case "japanese":
                        deck.DefaultFrontLang = Language.Japanese;
                        break;
                    case "german":
                        deck.DefaultFrontLang = Language.German;
                        break;
                    default:
                        return BadRequest("Current supported languages are English, Spanish, Japanese, and German");
                }
            }

            if (deckInfo.default_blang is not null)
            {
                switch (deckInfo.default_blang.ToLower())
                {
                    case "english":
                        deck.DefaultBackLang = Language.English;
                        break;
                    case "spanish":
                        deck.DefaultBackLang = Language.Spanish;
                        break;
                    case "japanese":
                        deck.DefaultBackLang = Language.Japanese;
                        break;
                    case "german":
                        deck.DefaultBackLang = Language.German;
                        break;
                    default:
                        return BadRequest("Current supported languages are English, Spanish, Japanese, and German");
                }
            }

            // Owner
            deck.UserId = _user.Id;
            
            // Dates
            deck.DateUpdated = DateTime.Now;

            // Save the deck
            _context.Decks.Update(deck);
            await _context.SaveChangesAsync();

            return Ok(new DeckUpdateResponse
            {
                id = deck.DeckID,
                dateUpdated = deck.DateUpdated
            });
        }

        // POST: api/Decks
        /// <summary>
        /// Creates a Deck for the currently authenticated user
        /// </summary>
        /// <param name="deckInfo">
        /// Required: title, description, default_flang, default_blang, userId -- Optional: access, cardIds
        /// </param>
        /// <remarks>Default access level is Private. The cardIds list currently does nothing.</remarks>
        /// <response code="201">Returns the id and creation date of the created Deck</response>
        /// <response code="400">Invalid input or input type</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="404">Object at cardId provided was not found</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(DeckCreationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostDeckCreate(PostDeckInfo deckInfo)
        {
            // Create and populate a deck with the given info
            Deck deck = new();
            
            if (String.IsNullOrEmpty(deckInfo.title))
            {
                return BadRequest("a non-empty title is required");
            }
            if(String.IsNullOrEmpty(deckInfo.description))
            {
                return BadRequest("a non-empty description is required");
            }
            if(String.IsNullOrEmpty(deckInfo.default_flang))
            {
                return BadRequest("a non-empty default_flang is required");
            }
            if(String.IsNullOrEmpty(deckInfo.default_blang))
            {
                return BadRequest("a non-empty default_blang is required");
            }

            // Add owner
            deck.UserId = _user.Id;

            /*
            IQueryable<dynamic> querycards;
            // Ensure all cards in deckInfo.cardIds exist and load them
            if(deckInfo.cardIds is not null)
            {
                querycards = from c in _context.Cards
                                 where deckInfo.cardIds.Contains(c.CardID)
                                 select c;

                if (querycards.Count() < deckInfo.cardIds.Count)
                {
                    List<int> cardIds = new();
                    foreach (Card card in querycards) { cardIds.Add(card.CardID); }
                    var querydiff = deckInfo.cardIds.Except(cardIds);
                    return NotFound("cardIds not found: " + JsonConvert.SerializeObject(querydiff));
                }
            }
            */

            //----------------Set the rest of the deck info
            deck.Title = deckInfo.title;
            deck.Description = deckInfo.description;
            // Handle the enums with switch cases
            if (deckInfo.access is null) deck.Access = Access.Private;
            else
            {
                switch (deckInfo.access.ToLower())
                {
                    case "public":
                        deck.Access = Access.Public;
                        break;
                    case "private":
                        deck.Access = Access.Private;
                        break;
                    default:
                        return BadRequest("access should be public, private, or omitted (default private)");
                }
            }

            switch (deckInfo.default_flang.ToLower())
            {
                case "english":
                    deck.DefaultFrontLang = Language.English;
                    break;
                case "spanish":
                    deck.DefaultFrontLang = Language.Spanish;
                    break;
                case "japanese":
                    deck.DefaultFrontLang = Language.Japanese;
                    break;
                case "german":
                    deck.DefaultFrontLang = Language.German;
                    break;
                default:
                    return BadRequest("Current supported languages are English, Spanish, Japanese, and German");
            }
            switch (deckInfo.default_blang.ToLower())
            {
                case "english":
                    deck.DefaultBackLang = Language.English;
                    break;
                case "spanish":
                    deck.DefaultBackLang = Language.Spanish;
                    break;
                case "japanese":
                    deck.DefaultBackLang = Language.Japanese;
                    break;
                case "german":
                    deck.DefaultBackLang = Language.German;
                    break;
                default:
                    return BadRequest("Current supported languages are English, Spanish, Japanese, and German");
            }

            // Dates
            deck.DateCreated = DateTime.Now;
            deck.DateTouched = DateTime.Now;
            deck.DateUpdated = DateTime.Now;

            await _context.Decks.AddAsync(deck);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostDeckCreate", new DeckCreationResponse
            {
                id = deck.DeckID,
                dateCreated = deck.DateCreated
            });
        }

        // DELETE: /Decks/Delete/{id}
        /// <summary>
        /// Deletes one specific deck owned by the currently authenticated user
        /// </summary>
        /// <param name="id">The deck's ID</param>
        /// <response code="204"></response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized for this action</response>
        /// <response code="404">Object at deckId was not found</response>
        [HttpPost("Delete/{id}")]
        [Produces("text/plain")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDeck(int id)
        {
            var deck = await _context.Decks.FindAsync(id);
            if (deck == null)
            {
                return NotFound("DeckId " + id + " was not found");
            }

            if(deck.UserId != _user.Id)
            {
                return Forbid();
            }

            _context.Decks.Remove(deck);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: /Decks/Delete?userId={userID}
        /// <summary>
        /// Delete all of one user's decks
        /// </summary>
        /// <param name="userId">The user's ID</param>
        /// <response code="204"></response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized for this action</response>
        /// <response code="404">Object at userId was not found</response>
        [HttpPost("Delete")]
        [Produces("text/plain")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUserDecks(string userId)
        {
            EchoUser user = await _um.FindByIdAsync(userId);
            if (user is null) return NotFound("UserId " + userId + " was not found");

            if(userId != _user.Id)
            {
                return Forbid();
            }

            var query = from d in _context.Decks
                        where d.UserId == user.Id
                        select d;

            List<Deck> userDecks = await query.ToListAsync();
            foreach (Deck deck in userDecks)
            {
                _context.Decks.Remove(deck);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
