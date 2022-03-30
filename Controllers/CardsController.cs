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
    public class CardsController : ControllerBase
    {
        private readonly EchoStudyDB _context;

        public CardsController(EchoStudyDB context)
        {
            _context = context;
        }

        public class CardInfo
        {
            public int id { get; set; }
            public string ftext { get; set; }
            public string btext { get; set; }
            public string faud { get; set; }
            public string baud { get; set; }
            public string flang { get; set; }
            public string blang { get; set; }
            public List<int> decks { get; set; }
            public int score { get; set; }
            public DateTime date_created { get; set; }
        }

        // GET: api/Cards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardInfo>>> GetCards()
        {
            var query = from c in _context.Cards
                        select new CardInfo
                        {
                            id = c.CardID,
                            ftext = c.FrontText,
                            btext = c.BackText,
                            faud = c.FrontAudio,
                            baud = c.BackAudio,
                            flang = c.FrontLang.ToString(),
                            blang = c.BackLang.ToString(),
                            decks = c.Decks.Select(d => d.DeckID).ToList(),
                            score = c.Score,
                            date_created = c.DateCreated
                        };
            return await query.ToListAsync();
        }

        // GET: api/Cards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CardInfo>> GetCard(int id)
        {
            var query = from c in _context.Cards
                        where c.CardID == id
                        select new CardInfo
                        {
                            id = c.CardID,
                            ftext = c.FrontText,
                            btext = c.BackText,
                            faud = c.FrontAudio,
                            baud = c.BackAudio,
                            flang = c.FrontLang.ToString(),
                            blang = c.BackLang.ToString(),
                            decks = c.Decks.Select(d => d.DeckID).ToList(),
                            score = c.Score,
                            date_created = c.DateCreated
                        };

            var card = query.FirstOrDefault();

            if (card == null)
            {
                return NotFound();
            }

            return card;
        }

        // PUT: api/Cards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCard(int id, Card card)
        {
            if (id != card.CardID)
            {
                return BadRequest();
            }

            _context.Entry(card).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CardExists(id))
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

        // POST: api/Cards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Card>> PostCard(Card card)
        {
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCard", new { id = card.CardID }, card);
        }

        // DELETE: api/Cards/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null)
            {
                return NotFound();
            }

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.CardID == id);
        }
    }
}
