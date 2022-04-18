using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using echoStudy_webAPI.Models;
using echoStudy_webAPI.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace echoStudy_webAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DeckCategoriesController : ControllerBase
    {
        private readonly EchoStudyDB _context;
        private readonly UserManager<EchoUser> _userManager;

        public DeckCategoriesController(EchoStudyDB context,
            UserManager<EchoUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /**
 * This class should contain all information needed in a GET request
 */
        public class DeckCategoryInfo
        {
            public int id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public List<int> decks { get; set; }
            public string ownerId { get; set; }
            public DateTime date_created { get; set; }
            public DateTime date_updated { get; set; }
            public DateTime date_touched { get; set; }
        }

        /**
         * This class should contain all information needed from the user to create a row in the database
         */
        public class PostDeckCategoryInfo
        {
            public string title { get; set; }
            public string description { get; set; }
            public string userEmail { get; set; }
            public List<int> deckIds { get; set; }
        }

        // GET: api/DeckCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeckCategoryInfo>>> GetDeckCategories()
        {
            var query = from dc in _context.DeckCategories
                        select new DeckCategoryInfo
                        {
                            id = dc.CategoryID,
                            title = dc.Title,
                            description = dc.Description,
                            decks = dc.Decks.Select(d => d.DeckID).ToList(),
                            ownerId = dc.UserId,
                            date_created = dc.DateCreated,
                            date_updated = dc.DateUpdated,
                            date_touched = dc.DateTouched
                        };
            return await query.ToListAsync();
        }

        // GET: api/DeckCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeckCategoryInfo>> GetDeckCategory(int id)
        {
            var query = from dc in _context.DeckCategories
                        where dc.CategoryID == id
                        select new DeckCategoryInfo
                        {
                            id = dc.CategoryID,
                            title = dc.Title,
                            description = dc.Description,
                            decks = dc.Decks.Select(d => d.DeckID).ToList(),
                            ownerId = dc.UserId,
                            date_created = dc.DateCreated,
                            date_updated = dc.DateUpdated,
                            date_touched = dc.DateTouched
                        };

            if (query.Count() == 0)
            {
                return NotFound();
            }

            return query.First();
        }

        /**
        * Gets all deck categories belonging to the given user provided by email
        */
        // GET: api/DeckCategories/UserEmail=johndoe@gmail.com
        [HttpGet("/DeckCategories/UserEmail={userEmail}")]
        public async Task<ActionResult<IEnumerable<DeckCategoryInfo>>> GetUserDeckCategoriesByEmail(string userEmail)
        {
            EchoUser user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                return BadRequest("User " + userEmail + " not found");
            }
            else
            {
                var query = from dc in _context.DeckCategories
                            where dc.UserId == user.Id
                            select new DeckCategoryInfo
                            {
                                id = dc.CategoryID,
                                title = dc.Title,
                                description = dc.Description,
                                decks = dc.Decks.Select(d => d.DeckID).ToList(),
                                ownerId = dc.UserId,
                                date_created = dc.DateCreated,
                                date_updated = dc.DateUpdated,
                                date_touched = dc.DateTouched
                            };
                return await query.ToListAsync();
            }
        }


        /**
         * Gets all deck categories belonging to the given user
         */
        // GET: api/DeckCategories/User=25c35795-ce2c-414a-ba58-152d475ba818
        [HttpGet("/DeckCategories/User={userId}")]
        public async Task<ActionResult<IEnumerable<DeckCategoryInfo>>> GetUserDeckCategories(string userId)
        {
            var query = from dc in _context.DeckCategories
                        where dc.UserId == userId
                        select new DeckCategoryInfo
                        {
                            id = dc.CategoryID,
                            title = dc.Title,
                            description = dc.Description,
                            decks = dc.Decks.Select(d => d.DeckID).ToList(),
                            ownerId = dc.UserId,
                            date_created = dc.DateCreated,
                            date_updated = dc.DateUpdated,
                            date_touched = dc.DateTouched
                        };
            return await query.ToListAsync();
        }

        /**
        * Gets the deck categories that owns a given deck id belonging to a given deck category
        */
        // GET: api/DeckCategories/GetDecks/categoryId=2
        [HttpGet("/DeckCategories/Deck={deckId}")]
        public async Task<ActionResult<IEnumerable<DeckCategoryInfo>>> GetOwnerDeckCategory(int deckId)
        {
            // Grab the deck. Only possible for one or zero results since ids are unique.
            var deckQuery = from d in _context.Decks
                            where d.DeckID == deckId
                            select d;
            if (deckQuery.Count() == 0)
            {
                return BadRequest("Deck id " + deckId + " does not exist");
            }
            else
            {
                Deck deck = deckQuery.First();
                var query = from dc in _context.DeckCategories
                            where dc.Decks.Contains(deck)
                            select new DeckCategoryInfo
                            {
                                id = dc.CategoryID,
                                title = dc.Title,
                                description = dc.Description,
                                decks = dc.Decks.Select(d => d.DeckID).ToList(),
                                ownerId = dc.UserId,
                                date_created = dc.DateCreated,
                                date_updated = dc.DateUpdated,
                                date_touched = dc.DateTouched
                            };
                return await query.ToListAsync();
            }
        }

        // PUT: api/DeckCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeckCategory(int id, PostDeckCategoryInfo deckCategoryInfo)
        {
            var deckCategoryQuery = from dc in _context.DeckCategories
                            where dc.CategoryID == id
                            select dc;
            // Create deck category
            if(deckCategoryQuery.Count() == 0)
            {
                // Create and populate a deck category with the given info
                DeckCategory deckCategory = new DeckCategory();
                deckCategory.Title = deckCategoryInfo.title;
                deckCategory.Description = deckCategoryInfo.description;

                // Associate the deck category with the appropriate decks
                deckCategory.Decks = new List<Deck>();
                foreach (int deckId in deckCategoryInfo.deckIds)
                {
                    // Grab the deck category. Only possible for one or zero results since ids are unique.
                    var query = from d in _context.Decks.Include(d => d.DeckCategories)
                                where d.DeckID == deckId
                                select d;
                    if (query.Count() == 0)
                    {
                        return BadRequest("Deck id " + deckId + " does not exist");
                    }
                    else
                    {
                        Deck deck = query.First();
                        deck.DeckCategories.Add(deckCategory);
                        deckCategory.Decks.Add(deck);
                        _context.Entry(deck).State = EntityState.Modified;
                    }
                }

                // Owner
                EchoUser user = await _userManager.FindByEmailAsync(deckCategoryInfo.userEmail);
                if (user is null)
                {
                    return BadRequest("User " + deckCategoryInfo.userEmail + " not found");
                }
                else
                {
                    deckCategory.UserId = user.Id;
                }

                // Dates
                deckCategory.DateCreated = DateTime.Now;
                deckCategory.DateTouched = DateTime.Now;
                deckCategory.DateUpdated = DateTime.Now;

                // Save the deck category
                _context.DeckCategories.Add(deckCategory);
                await _context.SaveChangesAsync();

                return CreatedAtAction("PutDeckCategory", new
                {
                    id = deckCategory.CategoryID,
                    dateCreated = deckCategory.DateCreated
                });
            }
            // Create the deck category
            else
            {
                DeckCategory deckCategory = deckCategoryQuery.First();

                // Owner
                EchoUser user = await _userManager.FindByEmailAsync(deckCategoryInfo.userEmail);
                if (user is null)
                {
                    return BadRequest("User " + deckCategoryInfo.userEmail + " not found");
                }
                else
                {
                    deckCategory.UserId = user.Id;
                }

                // Set description, title
                deckCategory.Title = deckCategoryInfo.title;
                deckCategory.Description = deckCategoryInfo.description;

                // Assign it to the cards given by id (if there is any)
                HashSet<Deck> updatedDecks = new HashSet<Deck>();
                foreach (int deckId in deckCategoryInfo.deckIds)
                {
                    // Grab the deck. Only possible for one or zero results since ids are unique.
                    var query = from d in _context.Decks.Include(d => d.DeckCategories)
                                where d.DeckID == deckId
                                select d;
                    if (query.Count() == 0)
                    {
                        return BadRequest("Deck id " + deckId + " does not exist");
                    }
                    else
                    {
                        Deck deck = query.First();
                        // If they aren't already related, relate them
                        if (!deck.DeckCategories.Contains(deckCategory))
                        {
                            deckCategory.Decks.Add(deck);
                            deck.DeckCategories.Add(deckCategory);
                        }
                        updatedDecks.Add(deck);
                        _context.Entry(deck).State = EntityState.Modified;
                    }
                }
                List<Deck> currentDecks = deckCategory.Decks.ToList();
                // Unrelate any decks if needed
                foreach (Deck deck in currentDecks)
                {
                    if (!updatedDecks.Contains(deck))
                    {
                        deck.DeckCategories.Remove(deckCategory);
                        deckCategory.Decks.Remove(deck);
                        _context.Entry(deck).State = EntityState.Modified;
                    }
                }

                // Change update date
                deckCategory.DateUpdated = DateTime.Now;

                // Mark the deck category as modified
                _context.Entry(deckCategory).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Failed to update deck category");
                }

                return Ok();
            }
        }

        // POST: api/DeckCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DeckCategory>> PostDeckCategory(PostDeckCategoryInfo deckCategoryInfo)
        {
            // Create and populate a deck category with the given info
            DeckCategory deckCategory = new DeckCategory();
            deckCategory.Title = deckCategoryInfo.title;
            deckCategory.Description = deckCategoryInfo.description;

            // Associate the deck category with the appropriate decks
            deckCategory.Decks = new List<Deck>();
            foreach (int deckId in deckCategoryInfo.deckIds)
            {
                // Grab the deck category. Only possible for one or zero results since ids are unique.
                var query = from d in _context.Decks.Include(d => d.DeckCategories)
                            where d.DeckID == deckId
                            select d;
                if (query.Count() == 0)
                {
                    return BadRequest("Deck id " + deckId + " does not exist");
                }
                else
                {
                    Deck deck = query.First();
                    deck.DeckCategories.Add(deckCategory);
                    deckCategory.Decks.Add(deck);
                    _context.Entry(deck).State = EntityState.Modified;
                }
            }

            // Owner
            EchoUser user = await _userManager.FindByEmailAsync(deckCategoryInfo.userEmail);
            if (user is null)
            {
                return BadRequest("User " + deckCategoryInfo.userEmail + " not found");
            }
            else
            {
                deckCategory.UserId = user.Id;
            }

            // Dates
            deckCategory.DateCreated = DateTime.Now;
            deckCategory.DateTouched = DateTime.Now;
            deckCategory.DateUpdated = DateTime.Now;

            // Save the deck category
            _context.DeckCategories.Add(deckCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostDeckCategory", new
            {
                id = deckCategory.CategoryID,
                dateCreated = deckCategory.DateCreated
            });
        }

        // DELETE: api/DeckCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeckCategory(int id)
        {
            var deckCategory = await _context.DeckCategories.FindAsync(id);
            if (deckCategory == null)
            {
                return NotFound();
            }

            _context.DeckCategories.Remove(deckCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /*
* Updates the given card by id 
*/
        // PATCH: api/Decks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchDeck(int id, PostDeckCategoryInfo deckCategoryInfo)
        {
            var deckCategoryQuery = from dc in _context.DeckCategories.Include(d => d.Decks)
                            where dc.CategoryID == id
                            select dc;
            // Create the card
            if (deckCategoryQuery.Count() == 0)
            {
                return BadRequest("Deck " + id + " does not exist");
            }
            // Update the card
            else
            {
                DeckCategory deckCategory = deckCategoryQuery.First();

                // Owner
                EchoUser user = await _userManager.FindByEmailAsync(deckCategoryInfo.userEmail);
                if (user is null)
                {
                    return BadRequest("User " + deckCategoryInfo.userEmail + " not found");
                }
                else
                {
                    deckCategory.UserId = user.Id;
                }

                // Set description, title
                deckCategory.Title = deckCategoryInfo.title;
                deckCategory.Description = deckCategoryInfo.description;

                // Assign it to the cards given by id (if there is any)
                HashSet<Deck> updatedDecks = new HashSet<Deck>();
                foreach (int deckId in deckCategoryInfo.deckIds)
                {
                    // Grab the deck. Only possible for one or zero results since ids are unique.
                    var query = from d in _context.Decks.Include(d => d.DeckCategories)
                                where d.DeckID == deckId
                                select d;
                    if (query.Count() == 0)
                    {
                        return BadRequest("Deck id " + deckId + " does not exist");
                    }
                    else
                    {
                        Deck deck = query.First();
                        // If they aren't already related, relate them
                        if (!deck.DeckCategories.Contains(deckCategory))
                        {
                            deckCategory.Decks.Add(deck);
                            deck.DeckCategories.Add(deckCategory);
                        }
                        updatedDecks.Add(deck);
                        _context.Entry(deck).State = EntityState.Modified;
                    }
                }
                List<Deck> currentDecks = deckCategory.Decks.ToList();
                // Unrelate any decks if needed
                foreach (Deck deck in currentDecks)
                {
                    if (!updatedDecks.Contains(deck))
                    {
                        deck.DeckCategories.Remove(deckCategory);
                        deckCategory.Decks.Remove(deck);
                        _context.Entry(deck).State = EntityState.Modified;
                    }
                }

                // Change update date
                deckCategory.DateUpdated = DateTime.Now;

                // Mark the deck category as modified
                _context.Entry(deckCategory).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Failed to update deck category");
                }

                return Ok();
            }
        }


        /*
        * Updates the given deck by id 
        */
        // PATCH: api/DeckCategories/Touch=1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("Touch={id}")]
        public async Task<IActionResult> TouchDeckCategory(int id)
        {
            var deckCategoryQuery = from dc in _context.DeckCategories
                            where dc.CategoryID == id
                            select dc;
            // Deck doesn't exist
            if (deckCategoryQuery.Count() == 0)
            {
                return BadRequest("Deck category " + id + " does not exist");
            }
            // Update the deck
            else
            {
                DeckCategory deckCategory = deckCategoryQuery.First();

                // Update touched date
                deckCategory.DateTouched = DateTime.Now;

                // Mark the card as modified
                _context.Entry(deckCategory).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok();
            }
        }

        /**
        * Deletes all deck categories associated with one user
        */
        // DELETE: /api/DeckCategories/DeleteUserDeckCategories=25c35795-ce2c-414a-ba58-152d475ba818
        [HttpDelete("/DeckCategories/DeleteUserDeckCategories={userId}")]
        public async Task<IActionResult> DeleteUserDecks(string userId)
        {
            var query = from dc in _context.DeckCategories
                        where dc.UserId == userId
                        select dc;

            List<DeckCategory> userDeckCategories = await query.ToListAsync();
            foreach (DeckCategory deckCategory in userDeckCategories)
            {
                _context.DeckCategories.Remove(deckCategory);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /**
         * Deletes all decks associated with one user
         */
        // DELETE: api/DeckCategories/DeleteUserDeckCategoriesByEmail=johnDoe@gmail.com
        [HttpDelete("/DeckCategories/DeleteUserDeckCategoriesByEmail={userEmail}")]
        public async Task<IActionResult> DeleteUserDecksByEmail(string userEmail)
        {
            EchoUser user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                return BadRequest("User " + userEmail + " not found");
            }
            else
            {
                var query = from dc in _context.DeckCategories
                            where dc.UserId == user.Id
                            select dc;

                List<DeckCategory> userDeckCategories = await query.ToListAsync();
                foreach (DeckCategory deckCategory in userDeckCategories)
                {
                    _context.DeckCategories.Remove(deckCategory);
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
        }

        private bool DeckCategoryExists(int id)
        {
            return _context.DeckCategories.Any(e => e.CategoryID == id);
        }
    }
}
