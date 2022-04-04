using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using echoStudy_webAPI.Models;
using echoStudy_webAPI.Data;
using Microsoft.AspNetCore.Identity;
using echoStudy_webAPI.Areas.Identity.Data;
using System.Net.Http;

namespace echoStudy_webAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly EchoStudyDB _context;
        private readonly UserManager<EchoUser> _userManager;

        public CardsController(EchoStudyDB context, UserManager<EchoUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /**
         * This class should contain all information that should be returned to the user in a GET request
         */
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
            public string ownerId { get; set; }
            public DateTime date_created { get; set; }
            public DateTime date_updated { get; set; }
            public DateTime date_touched { get; set; }
        }
        
        /**
         * This class should contain all information that should be provided in order to create or update a card
         */
        public class PostCardInfo
        {
            public string frontText { get; set; }
            public string backText { get; set; }
            public string frontLang { get; set; }
            public string backLang { get; set; }
            public string userEmail { get; set; }
            public List<int> deckIds { get; set; }
        }

        /**
         * Gets all cards
         */
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
                            ownerId = c.UserId,
                            date_created = c.DateCreated,
                            date_updated = c.DateUpdated,
                            date_touched = c.DateTouched
                        };
            return await query.ToListAsync();
        }

        /**
         * Gets all cards belonging to the given user provided by email
         */
        // GET: api/Cards/User=johndoe@gmail.com
        [HttpGet("/api/Cards/UserEmail={userEmail}")]
        public async Task<ActionResult<IEnumerable<CardInfo>>> GetUserCardsByEmail(string userEmail)
        {
            EchoUser user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                return BadRequest("User " + userEmail + " not found");
            }
            else
            {
                var query = from c in _context.Cards
                            where c.UserId == user.Id
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
                                ownerId = c.UserId,
                                date_created = c.DateCreated,
                                date_updated = c.DateUpdated,
                                date_touched = c.DateTouched
                            };
                return await query.ToListAsync();
            }
        }


        /**
         * Gets all cards belonging to the given user
         */
        // GET: api/Cards/User=johndoe@gmail.com
        [HttpGet("/api/Cards/User={userId}")]
        public async Task<ActionResult<IEnumerable<CardInfo>>> GetUserCards(string userId)
        {
                var query = from c in _context.Cards
                            where c.UserId == userId
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
                                ownerId = c.UserId,
                                date_created = c.DateCreated,
                                date_updated = c.DateUpdated,
                                date_touched = c.DateTouched
                            };
                return await query.ToListAsync();
        }

        /**
         * Gets all cards belonging to a given deck
         */
        // GET: api/Cards/Deck=2
        [HttpGet("/api/Cards/Deck={deckId}")]
        public async Task<ActionResult<IEnumerable<CardInfo>>> GetDeckCards(int deckId)
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
                var query = from c in _context.Cards
                            where c.Decks.Contains(deck)
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
                                ownerId = c.UserId,
                                date_created = c.DateCreated,
                                date_updated = c.DateUpdated,
                                date_touched = c.DateTouched
                            };
                return await query.ToListAsync();
            }
        }
        
        /**
         * Gets a single card given by id
         */
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
                            ownerId = c.UserId,
                            date_created = c.DateCreated,
                            date_updated = c.DateUpdated,
                            date_touched = c.DateTouched
                        };

            var card = query.FirstOrDefault();

            if (card == null)
            {
                return NotFound();
            }

            return card;
        }

        /*
 * "touches" given card by id
 */
        // PATCH: api/Cards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("Touch={id}&{score}")]
        public async Task<IActionResult> TouchCard(int id, int score)
        {
            var cardQuery = from c in _context.Cards.Include(c => c.Decks)
                            where c.CardID == id
                            select c;
            // Card doesn't exist
            if (cardQuery.Count() == 0)
            {
                return BadRequest("Card " + id + " does not exist");
            }
            // Update the card
            else
            {
                Card card = cardQuery.First();

                // Update score and last touched
                card.Score = score;
                card.DateTouched = DateTime.Now;

                // Mark the card as modified
                _context.Entry(card).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Card was successfully touched.", card });
            }
        }

        /*
         * Updates the given card by id 
         */
        // PATCH: api/Cards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCard(int id, PostCardInfo cardInfo)
        {
            var cardQuery = from c in _context.Cards.Include(c => c.Decks)
                        where c.CardID == id
                        select c;
            // Create the card
            if (cardQuery.Count() == 0)
            {
                return BadRequest("Card " + id + " does not exist");
            }
            // Update the card
            else
            {
                Card card = cardQuery.First();

                // Set the texts then set the audio
                if(card.FrontText != cardInfo.frontText)
                {
                    card.FrontText = cardInfo.frontText;
                    // TODO: Implement audio
                    card.FrontAudio = "todo";
                }
                if (card.BackText != cardInfo.backText)
                {
                    card.BackText = cardInfo.backText;
                    // TODO: Implement audio
                    card.BackAudio = "todo";
                }

                // Assign the owner
                EchoUser user = await _userManager.FindByEmailAsync(cardInfo.userEmail);
                if (user is null)
                {
                    return BadRequest("User " + cardInfo.userEmail + " not found");
                }
                else
                {
                    card.UserId = user.Id;
                }

                // Assign it to the decks given by id (if there is any)
                HashSet<Deck> updatedDecks = new HashSet<Deck>();
                foreach (int deckId in cardInfo.deckIds)
                {
                    // Grab the deck. Only possible for one or zero results since ids are unique.
                    var query = from d in _context.Decks.Include(d => d.Cards)
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
                        if (!card.Decks.Contains(deck))
                        {
                            card.Decks.Add(deck);
                            deck.Cards.Add(card);
                        }
                        updatedDecks.Add(deck);
                        _context.Entry(deck).State = EntityState.Modified;
                    }
                }
                List<Deck> currentDecks = card.Decks.ToList();
                // Unrelate any decks if needed
                foreach(Deck deck in currentDecks)
                {
                    if (!updatedDecks.Contains(deck))
                    {
                        card.Decks.Remove(deck);
                        deck.Cards.Remove(card);
                        _context.Entry(deck).State = EntityState.Modified;
                    }
                }

                // Change update date
                card.DateUpdated = DateTime.Now;

                // Mark the card as modified
                _context.Entry(card).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Failed to update card");
                }

                return Ok(new { message = "Card was successfully updated.", card });
            }
        }

        /**
         * Updates the card given by ID if it exists.
         * If the card doesn't exist, make it.
         */
        // PUT: api/Cards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCard(int id, PostCardInfo cardInfo)
        {
            var cardQuery = from c in _context.Cards.Include(c => c.Decks)
                            where c.CardID == id
                            select c;
            // Create the card
            if (cardQuery.Count() == 0)
            {
                // Create a card and assign it all of the basic provided data
                Card card = new Card();
                card.FrontText = cardInfo.frontText;
                card.BackText = cardInfo.backText;
                // TODO: Implement audio 
                card.FrontAudio = "todo";
                card.BackAudio = "todo";
                // frontLang and backLang are stored as enums so convert their string to the appropriate enum value.
                switch (cardInfo.frontLang.ToLower())
                {
                    case "english":
                        card.FrontLang = Language.English;
                        break;
                    case "spanish":
                        card.FrontLang = Language.Spanish;
                        break;
                    case "japanese":
                        card.FrontLang = Language.Japanese;
                        break;
                    case "german":
                        card.FrontLang = Language.German;
                        break;
                    default:
                        return BadRequest("Current supported languages are English, Spanish, Japanese, and German");
                }
                switch (cardInfo.backLang.ToLower())
                {
                    case "english":
                        card.BackLang = Language.English;
                        break;
                    case "spanish":
                        card.BackLang = Language.Spanish;
                        break;
                    case "japanese":
                        card.BackLang = Language.Japanese;
                        break;
                    case "german":
                        card.BackLang = Language.German;
                        break;
                    default:
                        return BadRequest("Current supported languages are English, Spanish, Japanese, and German");
                }
                // Have to retrieve the owner user by email
                EchoUser user = await _userManager.FindByEmailAsync(cardInfo.userEmail);
                if (user is null)
                {
                    return BadRequest("User " + cardInfo.userEmail + " not found");
                }
                else
                {
                    card.UserId = user.Id;
                }
                // Assign it to the decks given by id (if there is any)
                card.Decks = new List<Deck>();
                foreach (int deckId in cardInfo.deckIds)
                {
                    // Grab the deck. Only possible for one or zero results since ids are unique.
                    var query = from d in _context.Decks.Include(d => d.Cards)
                                where d.DeckID == deckId
                                select d;
                    if (query.Count() == 0)
                    {
                        return BadRequest("Deck id " + deckId + " does not exist");
                    }
                    else
                    {
                        Deck deck = query.First();
                        card.Decks.Add(deck);
                        deck.Cards.Add(card);
                        _context.Entry(deck).State = EntityState.Modified;
                    }
                }
                // Assign it dates and a score of 0
                card.DateCreated = DateTime.Now;
                card.DateTouched = DateTime.Now;
                card.DateUpdated = DateTime.Now;
                card.Score = 0;
                _context.Cards.Add(card);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCard", new { id = card.CardID }, card);
            }
            // Update the card
            else
            {
                Card card = cardQuery.First();

                // Set the texts then set the audio
                if (card.FrontText != cardInfo.frontText)
                {
                    card.FrontText = cardInfo.frontText;
                    // TODO: Implement audio
                    card.FrontAudio = "todo";
                }
                if (card.BackText != cardInfo.backText)
                {
                    card.BackText = cardInfo.backText;
                    // TODO: Implement audio
                    card.BackAudio = "todo";
                }

                // Assign the owner
                EchoUser user = await _userManager.FindByEmailAsync(cardInfo.userEmail);
                if (user is null)
                {
                    return BadRequest("User " + cardInfo.userEmail + " not found");
                }
                else
                {
                    card.UserId = user.Id;
                }

                // Assign it to the decks given by id (if there is any)
                HashSet<Deck> updatedDecks = new HashSet<Deck>();
                foreach (int deckId in cardInfo.deckIds)
                {
                    // Grab the deck. Only possible for one or zero results since ids are unique.
                    var query = from d in _context.Decks.Include(d => d.Cards)
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
                        if (!card.Decks.Contains(deck))
                        {
                            card.Decks.Add(deck);
                            deck.Cards.Add(card);
                        }
                        updatedDecks.Add(deck);
                        _context.Entry(deck).State = EntityState.Modified;
                    }
                }
                List<Deck> currentDecks = card.Decks.ToList();
                // Unrelate any decks if needed
                foreach (Deck deck in currentDecks)
                {
                    if (!updatedDecks.Contains(deck))
                    {
                        card.Decks.Remove(deck);
                        deck.Cards.Remove(card);
                        _context.Entry(deck).State = EntityState.Modified;
                    }
                }

                // Change update date
                card.DateUpdated = DateTime.Now;

                // Mark the card as modified
                _context.Entry(card).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Failed to update card");
                }

                return Ok(new { message = "Card was successfully updated.", card });
            }
        }

        /**
         * Creates a brand new card 
         */
        // POST: api/Cards
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Card>> PostCard(PostCardInfo cardInfo)
        {
            // Create a card and assign it all of the basic provided data
            Card card = new Card();
            card.FrontText = cardInfo.frontText;
            card.BackText = cardInfo.backText;
            // TODO: Implement audio 
            card.FrontAudio = "todo";
            card.BackAudio = "todo";
            // frontLang and backLang are stored as enums so convert their string to the appropriate enum value.
            switch (cardInfo.frontLang.ToLower())
            {
                case "english":
                    card.FrontLang = Language.English;
                    break;
                case "spanish":
                    card.FrontLang = Language.Spanish;
                    break;
                case "japanese":
                    card.FrontLang = Language.Japanese;
                    break;
                case "german":
                    card.FrontLang = Language.German;
                    break;
                default:
                    return BadRequest("Current supported languages are English, Spanish, Japanese, and German");
            }
            switch (cardInfo.backLang.ToLower())
            {
                case "english":
                    card.BackLang = Language.English;
                    break;
                case "spanish":
                    card.BackLang = Language.Spanish;
                    break;
                case "japanese":
                    card.BackLang = Language.Japanese;
                    break;
                case "german":
                    card.BackLang = Language.German;
                    break;
                default:
                    return BadRequest("Current supported languages are English, Spanish, Japanese, and German");
            }
            // Have to retrieve the owner user by email
            EchoUser user = await _userManager.FindByEmailAsync(cardInfo.userEmail);
            if(user is null)
            {
                return BadRequest("User " + cardInfo.userEmail + " not found");
            }
            else
            {
                card.UserId = user.Id;
            }
            // Assign it to the decks given by id (if there is any)
            card.Decks = new List<Deck>();
            foreach(int deckId in cardInfo.deckIds)
            {
                // Grab the deck. Only possible for one or zero results since ids are unique.
                var query = from d in _context.Decks.Include(d => d.Cards)
                            where d.DeckID == deckId
                            select d;
                if(query.Count() == 0)
                {
                    return BadRequest("Deck id " + deckId + " does not exist");
                }
                else
                {
                    Deck deck = query.First();
                    card.Decks.Add(deck);
                    deck.Cards.Add(card);
                    _context.Entry(deck).State = EntityState.Modified;
                }
            }
            // Assign it dates and a score of 0
            card.DateCreated = DateTime.Now;
            card.DateTouched = DateTime.Now;
            card.DateUpdated = DateTime.Now;
            card.Score = 0;
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCard", new { id = card.CardID }, card);
        }

        /** 
         * Deletes given card by id
         */
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

        /**
         * Deletes all cards associated with one user
         */
        // DELETE: api/Cards/5
        [HttpDelete("/api/Cards/DeleteUserCards={userId}")]
        public async Task<IActionResult> DeleteUserCards(string userId)
        {
            var query = from c in _context.Cards
                        where c.UserId == userId
                        select c;

            List<Card> userCards = await query.ToListAsync();
            foreach(Card card in userCards)
            {
                _context.Cards.Remove(card);
            }
            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /**
 * Deletes all cards associated with one user
 */
        // DELETE: api/Cards/5
        [HttpDelete("/api/Cards/DeleteUserCardsByEmail={userEmail}")]
        public async Task<IActionResult> DeleteUserCardsByEmail(string userEmail)
        {
            EchoUser user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                return BadRequest("User " + userEmail + " not found");
            }
            else
            {
                var query = from c in _context.Cards
                            where c.UserId == user.Id
                            select c;

                List<Card> userCards = await query.ToListAsync();
                foreach (Card card in userCards)
                {
                    _context.Cards.Remove(card);
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
        }

        /**
         * Deletes all cards associated with a deck
         */
        [HttpDelete("/api/Cards/DeleteDeckCards={deckId}")]
        public async Task<IActionResult> DeleteDeckCards(int deckId)
        {
            // Grab the deck. Only possible for one or zero results since ids are unique.
            var deckQuery = from d in _context.Decks.Include(d => d.Cards)
                        where d.DeckID == deckId
                        select d;
            if (deckQuery.Count() == 0)
            {
                return BadRequest("Deck id " + deckId + " does not exist");
            }

            Deck deck = deckQuery.First();

            foreach (Card card in deck.Cards)
            {
                _context.Cards.Remove(card);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.CardID == id);
        }
    }
}
