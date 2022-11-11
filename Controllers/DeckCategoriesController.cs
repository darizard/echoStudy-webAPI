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
using Microsoft.AspNetCore.Mvc.Filters;
using Castle.Core.Internal;
using static echoStudy_webAPI.Controllers.CardsController;

namespace echoStudy_webAPI.Controllers
{
    [Route("deckcategories")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi=true)]
    public class DeckCategoriesController : EchoUserControllerBase
    {
        private readonly EchoStudyDB _context;
        private readonly UserManager<EchoUser> _userManager;

        public DeckCategoriesController(EchoStudyDB context, UserManager<EchoUser> um) : base(um)
        {
            _context = context;
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
            public List<int> deckIds { get; set; }
            public int? categoryId { get; set; }
        }

        /**
        * Define response type for category update
        */
        public class DeckCategoryUpdateResponse
        {
            public List<int> ids { get; set; }
            public DateTime dateUpdated { get; set; }
        }

        /**
        * Define response type for category creation
        */
        public class DeckCategoryCreationResponse
        {
            public List<int> ids { get; set; }
            public DateTime dateCreated { get; set; }
        }

        // GET: /deckcategories
        /// <summary>
        /// Retrieves all Deck Categories owned by the authenticated user or a specific deck category by id
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="id">The ID of the related deck category</param>"
        /// <response code="200">Returns the queried Deck Category objects</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to access the specified category</response>
        /// <response code="404">Object not found with the provided id</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<DeckCategoryInfo>>> GetDeckCategory(int? id)
        {
            List<DeckCategoryInfo> categories = new List<DeckCategoryInfo>();

            // Grab one specific category if the id is provided
            if(id is not null)
            {
                var querycategory = from dc in _context.DeckCategories.Include(d => d.Decks)
                                    where dc.CategoryID == id
                                    select dc;
                // Ensure it exists
                if (querycategory.IsNullOrEmpty())
                {
                    return NotFound();
                }
                DeckCategory category = await querycategory.FirstAsync();
                // Ensure they own it
                if (category.UserId != _user.Id)
                {
                    return Forbid();
                }
                // It's good to grab
                categories.Add(new DeckCategoryInfo
                {
                    id = category.CategoryID,
                    title = category.Title,
                    description = category.Description,
                    decks = category.Decks.Select(d => d.DeckID).ToList(),
                    ownerId = category.UserId,
                    date_created = category.DateCreated,
                    date_updated = category.DateUpdated
                });
            }
            // Grab everything the user owns otherwise
            else
            {
                var querycategory = from dc in _context.DeckCategories.Include(d => d.Decks)
                                    where dc.UserId == _user.Id
                                    select new DeckCategoryInfo 
                                    {
                                        id = dc.CategoryID,
                                        title = dc.Title,
                                        description = dc.Description,
                                        decks = dc.Decks.Select(d => d.DeckID).ToList(),
                                        ownerId = dc.UserId,
                                        date_created = dc.DateCreated,
                                        date_updated = dc.DateUpdated
                                    };
                categories = await querycategory.ToListAsync();
            }

            return Ok(categories);
        }

        // POST: /deckcategories
        /// <summary>
        /// Creates deck categories for the currently authenticated user through the provided list
        /// </summary>
        /// <param name="categoryInfos">
        /// List of cards to create.
        /// Required: title, description, deckIds
        /// </param>
        /// <remarks></remarks>
        /// <response code="201">Returns the ids and creation date of the resulting deck categories</response>
        /// <response code="400">Invalid input or input type</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to add a deck category to the specified deck</response>
        /// <response code="404">Deck category at deckId provided was not found</response>
        /// <response code="500">Database failed to complete deck category creation despite valid request</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CardCreationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostDeckCategory(List<PostDeckCategoryInfo> categoryInfos)
        {
            List<int> createdCategories = new List<int>();

            foreach (PostDeckCategoryInfo categoryInfo in categoryInfos)
            {
                // Required fields
                if (categoryInfo.title is null)
                {
                    return BadRequest("title is required at index " + createdCategories.Count);
                }
                if (categoryInfo.description is null)
                {
                    return BadRequest("description is required at index " + createdCategories.Count);
                }
                if (categoryInfo.deckIds is null)
                {
                    return BadRequest("deckIds is required at index " + createdCategories.Count);
                }

                // Everything required is present so create the category
                DeckCategory category = new DeckCategory
                {
                    Title = categoryInfo.title,
                    Description = categoryInfo.description,
                    UserId = _user.Id,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    DateTouched = DateTime.Now
                };

                // Grab the related decks
                var querydeck = from d in _context.Decks
                                where categoryInfo.deckIds.Contains(d.DeckID)
                                select d;
                category.Decks = await querydeck.ToListAsync();

                // Ready to add
                _context.DeckCategories.Add(category);
                createdCategories.Add(category.CategoryID);
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

            return CreatedAtAction("PostDeckCategory", new DeckCategoryCreationResponse
            {
                ids = createdCategories,
                dateCreated = DateTime.Now
            });
        }

        // Post: /deckcategories/update
        /// <summary>
        /// Updates deck categories provided through a list
        /// </summary>
        /// <remarks></remarks>
        /// <param name="categoryInfos">
        /// List of categories to be updated and their data
        /// Optional: title, description, deckIds
        /// </param>
        /// <response code="200">Returns the Ids of each category and DateUpdated</response>
        /// <response code="400">Invalid input or input type</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to access the specified deck category</response>
        /// <response code="404">Object at the categoryId provided was not found</response>
        /// <response code="500">Database failed to complete category update despite valid request</response>
        [HttpPost("update")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(typeof(CardUpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostCategoryEdit(List<PostDeckCategoryInfo> categoryInfos)
        {
            List<int> ids = new List<int>();

            foreach (PostDeckCategoryInfo categoryInfo in categoryInfos)
            {
                // Ensure ID was provided
                if (categoryInfo.categoryId is null)
                {
                    return BadRequest("Category ID is required for editing");
                }
                ids.Add((int)categoryInfo.categoryId);

                // Ensure the category exists and that the owner made the request
                DeckCategory category = await _context.DeckCategories.FindAsync(categoryInfo.categoryId);
                if (category is null)
                {
                    return NotFound("Category id " + categoryInfo.categoryId + " not found");
                }
                if (category.UserId != _user.Id)
                {
                    return Forbid();
                }

                //-------Update according to incoming info
                if(categoryInfo.title is not null)
                {
                    category.Title = categoryInfo.title;
                }
                if (categoryInfo.description is not null)
                {
                    category.Description = categoryInfo.description;
                }
                if (categoryInfo.deckIds is not null)
                {
                    var querydeck = from d in _context.Decks
                                    where categoryInfo.deckIds.Contains(d.DeckID)
                                    select d;

                    category.Decks = await querydeck.ToListAsync();
                }

                // Modify the updated date
                category.DateUpdated = DateTime.Now;

                // Indicate to the DB this category has changed
                _context.DeckCategories.Update(category);
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
            return Ok(new DeckCategoryUpdateResponse
            {
                ids = ids,
                dateUpdated = DateTime.Now
            });
        }

        // DELETE: /deckcategories/delete
        /// <summary>
        /// Deletes the deck categories related to the provided IDs
        /// </summary>
        /// <param name="ids">List of IDs of categories to be deleted</param>
        /// <response code="204">Provided deck categories were successfully deleted</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to access the specified card</response>
        /// <response code="404">Object at cardId was not found</response>
        /// <response code="500">Database failed to complete card deletion despite valid request</response>
        [HttpPost("delete")]
        [Produces("text/plain", "application/json")]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostCategoryDelete(List<int> ids)
        {
            // Delete the category
            foreach (int id in ids)
            {
                DeckCategory category = await _context.DeckCategories.FindAsync(id);
                if (category == null)
                {
                    return NotFound();
                }
                if (category.UserId != _user.Id)
                {
                    return Forbid();
                }
                _context.DeckCategories.Remove(category);
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

            return NoContent();
        }

        private bool DeckCategoryExists(int id)
        {
            return _context.DeckCategories.Any(e => e.CategoryID == id);
        }
    }
}
