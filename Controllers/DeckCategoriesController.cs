using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using echoStudy_webAPI.Models;

namespace echoStudy_webAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeckCategoriesController : ControllerBase
    {
        private readonly EchoStudyDB _context;

        public DeckCategoriesController(EchoStudyDB context)
        {
            _context = context;
        }

        // GET: api/DeckCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeckCategory>>> GetDeckCategories()
        {
            return await _context.DeckCategories.ToListAsync();
        }

        // GET: api/DeckCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeckCategory>> GetDeckCategory(int id)
        {
            var deckCategory = await _context.DeckCategories.FindAsync(id);

            if (deckCategory == null)
            {
                return NotFound();
            }

            return deckCategory;
        }

        // PUT: api/DeckCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeckCategory(int id, DeckCategory deckCategory)
        {
            if (id != deckCategory.CategoryID)
            {
                return BadRequest();
            }

            _context.Entry(deckCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeckCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DeckCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DeckCategory>> PostDeckCategory(DeckCategory deckCategory)
        {
            _context.DeckCategories.Add(deckCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeckCategory", new { id = deckCategory.CategoryID }, deckCategory);
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

        private bool DeckCategoryExists(int id)
        {
            return _context.DeckCategories.Any(e => e.CategoryID == id);
        }
    }
}
