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
            public List<int> cardIds { get; set; }
        }

        // GET: /Decks
        /// <summary>
        /// Retrieves all Deck objects, or optionally deck objects related
        /// to a user by Id or by Email. If no parameter is specified, returns all deck objects.
        /// If userId or userEmail is specified, returns the decks related to the given user. If
        /// both parameters are specified, userId takes precedence.
        /// </summary>
        /// <param name="userId">The ASP.NET Id of the related user</param>
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
        /// Retrieves all Deck objects which have an access level of Public
        /// </summary>
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

        /**
         * Gets a deck given by id
         */
        // GET: /Decks/5
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

        /**
        * Gets all decks belonging to the given user provided by email
         */
        // GET: /Decks/ByEmail?userEmail=johndoe@gmail.com
        [HttpGet("ByEmail")]
        public async Task<ActionResult<IEnumerable<DeckInfo>>> GetUserDecksByEmail(string userEmail)
        {
            EchoUser user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                return NotFound("User " + userEmail + " not found");
            }
            else
            {
                var query = from d in _context.Decks
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
                return await query.ToListAsync();
            }
        }


        /**
         * Gets all cards belonging to the given user
         */
        // GET: /Decks/ByUserId?userId=userId
        [HttpGet("/Decks/ByUserId={userId}")]
        public async Task<ActionResult<IEnumerable<DeckInfo>>> GetUserDecks(string userId)
        {
            var query = from d in _context.Decks
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
            return await query.ToListAsync();
        }

        /**
        * Gets the deck category that owns a given deck id belonging to a given deck category
        */
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

        // PUT: api/Decks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeck(int id, PostDeckInfo deckInfo)
        {
            var deckQuery = from d in _context.Decks
                            where d.DeckID == id
                            select d;
            // Create the deck
            if (!deckQuery.Any())
            {
                // Create and populate a deck with the given info
                Deck deck = new Deck();
                deck.Title = deckInfo.title;
                deck.Description = deckInfo.description;
                // Handle the enums with switch cases
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
                // Associate the deck with the appropriate cards
                deck.Cards = new List<Card>();
                foreach (int cardId in deckInfo.cardIds)
                {
                    // Grab the deck. Only possible for one or zero results since ids are unique.
                    var query = from c in _context.Cards
                                where c.CardID == cardId
                                select c;
                    if (!query.Any())
                    {
                        return BadRequest("Deck id " + cardId + " does not exist");
                    }
                    else
                    {
                        Card card = query.First();
                        card.Deck = deck;
                        deck.Cards.Add(card);
                        _context.Entry(card).State = EntityState.Modified;
                    }
                }
                // Owner
                EchoUser user = await _userManager.FindByEmailAsync(deckInfo.userEmail);
                if (user is null)
                {
                    return BadRequest("User " + deckInfo.userEmail + " not found");
                }
                else
                {
                    deck.UserId = user.Id;
                }
                // Dates
                deck.DateCreated = DateTime.Now;
                deck.DateTouched = DateTime.Now;
                deck.DateUpdated = DateTime.Now;

                // Save the deck
                _context.Decks.Add(deck);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetDeck", new { id = deck.DeckID }, deck);
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
                    return BadRequest("Failed to update card");
                }

                return Ok(new { message = "Card was successfully updated.", deck });
            }
        }

        // POST: api/Decks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Deck>> PostDeck(PostDeckInfo deckInfo)
        {
            
            // Create and populate a deck with the given info
            Deck deck = new Deck();
            deck.Title = deckInfo.title;
            deck.Description = deckInfo.description;
            // Handle the enums with switch cases
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
            // Associate the deck with the appropriate cards
            deck.Cards = new List<Card>();
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
                    card.CardID = deck.DeckID;
                    deck.Cards.Add(card);
                    _context.Entry(card).State = EntityState.Modified;
                }
            }
            // Owner
            EchoUser user = await _userManager.FindByEmailAsync(deckInfo.userEmail);
            if (user is null)
            {
                return BadRequest("User " + deckInfo.userEmail + " not found");
            }
            else
            {
                deck.UserId = user.Id;
            }
            // Dates
            deck.DateCreated = DateTime.Now;
            deck.DateTouched = DateTime.Now;
            deck.DateUpdated = DateTime.Now;

            // Save the deck
            _context.Decks.Add(deck);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeck", new { id = deck.DeckID }, deck);
        }

        /*
 * Updates the given deck by id 
 */
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

                return Ok(new { message = "Deck was successfully updated.", deck });
            }
        }

        /*
        * Updates the given deck by id 
        */
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

                return Ok(new { message = "Deck was successfully touched.", deck });
            }
        }

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
        // DELETE: api/Cards/5
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

        private bool DeckExists(int id)
        {
            return _context.Decks.Any(e => e.DeckID == id);
        }
    }
}
