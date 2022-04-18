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
using Newtonsoft.Json;

namespace echoStudy_webAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly EchoStudyDB _context;
        private readonly UserManager<EchoUser> _userManager;

        public DecksController(EchoStudyDB context, UserManager<EchoUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        // GET: /Decks
        /// <summary>
        /// Retrieves all Deck objects or deck objects related to a user by Id or by Email
        /// </summary>
        /// <remarks>If no parameter is specified, returns all deck objects.
        /// If userId or userEmail is specified, returns the decks related to the given user. If
        /// both parameters are specified, userId takes precedence.
        /// </remarks>
        /// <param name="userId">The ASP.NET Id of the related user. Overrides <c>userEmail</c> if present</param>
        /// <param name="userEmail">The email address of the related user</param>
        /// <returns>A JSON list of Deck objects</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeckInfo>>> GetDecks(string userId, string userEmail)
        {
            if(userId != null)
            { 
                var queryid = from d in _context.Decks
                            where d.UserId == userId
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
                return await queryid.ToListAsync();
            }

            if(userEmail != null)
            {
                EchoUser user = await _userManager.FindByEmailAsync(userEmail);

                var queryid = from d in _context.Decks
                              where d.UserId == user.Id
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
                return await queryid.ToListAsync();
            }

            var queryall = from d in _context.Decks
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
            return await queryall.ToListAsync();
        }

        // GET: /Decks/Public
        /// <summary>
        /// Retrieves Public Decks
        /// </summary>
        /// <remarks>
        /// All Decks with an access level of Public
        /// </remarks>
        /// <returns>A JSON list of Deck objects</returns>
        [HttpGet("Public")]
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
            return await query.ToListAsync();
        }

        // GET: /Decks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DeckInfo>> GetDeck(int id)
        {
            var query = from d in _context.Decks
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

            if (!query.Any())
            {
                return NotFound();
            }

            return await query.FirstAsync();
        }

        //-------------------Keep commented method for when we are using JWTs? Can query by category here
        /**
        * Gets the deck category that owns a given deck id belonging to a given deck category
        */
        /*
        // GET: api/Decks/DeckCategory=2
        [HttpGet("/Decks/DeckCategory={categoryId}")]
        public async Task<ActionResult<IEnumerable<DeckInfo>>> GetDeckCategoryDecks(int categoryId)
        {
            // Grab the deck category. Only possible for one or zero results since ids are unique.
            var deckCategoryQuery = from dc in _context.DeckCategories
                            where dc.CategoryID == categoryId
                            select dc;
            if (deckCategoryQuery.Count() == 0)
            {
                return BadRequest("Category id " + categoryId + " does not exist");
            }
            else
            {
                DeckCategory deckCategory = deckCategoryQuery.First();
                var query = from d in _context.Decks
                            where d.DeckCategories.Contains(deckCategory)
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
                return await query.ToListAsync();
            }
        }
        */

        // POST: /Decks/{id}
        /// <summary>
        /// Edits an existing Deck
        /// </summary>
        /// <remarks>
        /// Currently does not support changing its associated cards
        /// </remarks>
        /// <param name="id">ID of the Deck to edit</param>
        /// <param name="deckInfo">
        /// Optional: title, description, default_flang, default_blang, userId, access, cardIds
        /// </param>
        /// <returns>A JSON list of Deck objects</returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> PostDeckEdit(int id, PostDeckInfo deckInfo)
        {
            Deck deck;
            var deckQuery = from d in _context.Decks
                            where d.DeckID == id
                            select d;
            if((deck = deckQuery.First()) is null) return NotFound("Deck id " + id + " not found");

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
            if(deckInfo.userId is not null)
            {
                EchoUser user = await _userManager.FindByIdAsync(deckInfo.userId);
                if (user is null) return BadRequest("User " + deckInfo.userId + " not found");

                deck.UserId = user.Id;
            }

            // Dates
            deck.DateUpdated = DateTime.Now;

            // Save the deck
            _context.Decks.Update(deck);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Decks
        /// <summary>
        /// Creates a Deck for a specific user
        /// </summary>
        /// <param name="deckInfo">
        /// Required: title, description, default_flang, default_blang, userId -- Optional: access, cardIds
        /// </param>
        /// <remarks>Default access level is Private. The cardIds list currently does nothing.</remarks>
        /// <returns>The id and creation date of the new deck</returns>
        [HttpPost]
        public async Task<ActionResult<Deck>> PostDeckCreate(PostDeckInfo deckInfo)
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
            if(String.IsNullOrEmpty(deckInfo.userId))
            {
                return BadRequest("a non-empty userId is required");
            }

            // Ensure Owner exists and add it
            EchoUser user = await _userManager.FindByIdAsync(deckInfo.userId);
            if (user is null) { return BadRequest("User " + deckInfo.userId + " not found"); }
            deck.UserId = user.Id;

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

            //----------------Set the rest of the deck info
            deck.Title = deckInfo.title;
            deck.Description = deckInfo.description;
            // Handle the enums with switch cases
            switch (deckInfo.access.ToLower())
            {
                case "public":
                    deck.Access = Access.Public;
                    break;
                default:
                    deck.Access = Access.Private;
                    break;
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

            return CreatedAtAction("PostDeckCreate", new
            {
                id = deck.DeckID,
                dateCreated = deck.DateCreated
            });
        }

        /*
 * Updates the given deck by id 
 */
        /*
        // PATCH: api/Decks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchDeck(int id, PostDeckInfo deckInfo)
        {
            var deckQuery = from d in _context.Decks.Include(d => d.Cards)
                            where d.DeckID == id
                            select d;
            // Create the deck
            if (deckQuery.Count() == 0)
            {
                return BadRequest("Deck " + id + " does not exist");
            }
            // Update the deck
            else
            {
                Deck deck = deckQuery.First();

                // Assign the owner
                EchoUser user = await _userManager.FindByEmailAsync(deckInfo.userEmail);
                if (user is null)
                {
                    return BadRequest("User " + deckInfo.userEmail + " not found");
                }
                else
                {
                    deck.UserId = user.Id;
                }

                // Set description, title
                deck.Title = deckInfo.title;
                deck.Description = deckInfo.description;

                // Set the enums
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

                // Assign it to the cards given by id (if there is any)
                HashSet<Card> updatedCards = new HashSet<Card>();
                foreach (int cardId in deckInfo.cardIds)
                {
                    // Grab the deck. Only possible for one or zero results since ids are unique.
                    var query = from c in _context.Cards
                                where c.CardID == cardId
                                select c;
                    if (query.Count() == 0)
                    {
                        return BadRequest("Deck id " + cardId + " does not exist");
                    }
                    else
                    {
                        Card card = query.First();
                        // If they aren't already related, relate them
                        if (!deck.Cards.Contains(card))
                        {
                            card.Deck = deck;
                            deck.Cards.Add(card);
                        }
                        updatedCards.Add(card);
                        _context.Entry(card).State = EntityState.Modified;
                    }
                }

                // Change update date
                deck.DateUpdated = DateTime.Now;

                // Mark the deck as modified
                _context.Entry(deck).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Failed to update deck");
                }

                return Ok(new { deck.DeckID });
            }
        }
        */

        /*
        * Updates the given deck by id 
        */
        /*
        // PATCH: api/Decks/Touch=1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("Touch={id}")]
        public async Task<IActionResult> TouchDeck(int id)
        {
            var deckQuery = from d in _context.Decks
                            where d.DeckID == id
                            select d;
            // Deck doesn't exist
            if (deckQuery.Count() == 0)
            {
                return BadRequest("Deck " + id + " does not exist");
            }
            // Update the deck
            else
            {
                Deck deck = deckQuery.First();

                // Update touched date
                deck.DateTouched = DateTime.Now;

                // Mark the card as modified
                _context.Entry(deck).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { id = Deck.DeckID });
            }
        }
        */

        
        // DELETE: api/Decks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeck(int id)
        {
            var deck = await _context.Decks.FindAsync(id);
            if (deck == null)
            {
                return NotFound();
            }

            _context.Decks.Remove(deck);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        

        /**
        * Deletes all decks associated with one user
        */
        // DELETE: api/Decks/5
        [HttpDelete("/Decks/DeleteUserDecks={userId}")]
        public async Task<IActionResult> DeleteUserDecks(string userId)
        {
            var query = from d in _context.Decks
                        where d.UserId == userId
                        select d;

            List<Deck> userDecks = await query.ToListAsync();
            foreach (Deck deck in userDecks)
            {
                _context.Decks.Remove(deck);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /**
         * Deletes all decks associated with one user
         */
        /*
        // DELETE: api/Decks/DeleteUserDecksByEmail=johnDoe@gmail.com
        [HttpDelete("/Decks/DeleteUserDecksByEmail={userEmail}")]
        public async Task<IActionResult> DeleteUserDecksByEmail(string userEmail)
        {
            EchoUser user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                return BadRequest("User " + userEmail + " not found");
            }
            else
            {
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
        */

        private bool DeckExists(int id)
        {
            return _context.Decks.Any(e => e.DeckID == id);
        }
    }
}
