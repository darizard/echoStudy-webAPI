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
    public class DecksController : ControllerBase
    {
        private readonly EchoStudyDB _context;

        public DecksController(EchoStudyDB context)
        {
            _context = context;
        }

       // class DeckInfo { }

        public class DeckInfo
        {
            public int id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string access { get; set; }
            public string default_flang { get; set; }
            public string default_blang { get; set; }
            public List<int> cards { get; set; }
            public DateTime creation_date { get; set; }
        }

        // GET: api/Decks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeckInfo>>> GetDecks()
        {
            var query = from deck in _context.Decks
                        select new DeckInfo
                        {
                            id = deck.DeckID,
                            title = deck.Title,
                            description = deck.Description,
                            access = deck.Access.ToString(),
                            default_flang = deck.DefaultFrontLang.ToString(),
                            default_blang = deck.DefaultBackLang.ToString(),
                            cards = deck.Cards.Select(c => c.CardID).ToList(),
                            creation_date = deck.DateCreated
                        } ;
            return await query.ToListAsync();
        }

        // GET: api/Decks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Deck>> GetDeck(int id)
        {
            var deck = await _context.Decks.FindAsync(id);

            if (deck == null)
            {
                return NotFound();
            }

            return deck;
        }

        // PUT: api/Decks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeck(int id, Deck deck)
        {
            if (id != deck.DeckID)
            {
                return BadRequest();
            }

            _context.Entry(deck).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeckExists(id))
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

        // POST: api/Decks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Deck>> PostDeck(Deck deck)
        {
            _context.Decks.Add(deck);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeck", new { id = deck.DeckID }, deck);
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

        private bool DeckExists(int id)
        {
            return _context.Decks.Any(e => e.DeckID == id);
        }
    }
}
