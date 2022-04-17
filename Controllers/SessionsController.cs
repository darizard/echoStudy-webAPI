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

namespace echoStudy_webAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly EchoStudyDB _context;
        private readonly UserManager<EchoUser> _userManager;

        public SessionsController(EchoStudyDB context, UserManager<EchoUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /**
         * This class should contain all information needed in a GET request
         */
        public class SessionInfo
        {
            public int id { get; set; }
            public string play_order { get; set; }
            public string learn_review { get; set; }
            public int max_cards { get; set; }
            public string platform { get; set; }
            public string device { get; set; }
            public string ownerId { get; set; }
            public int deckId { get; set; }
            public List<int> cards_played { get; set; }
            public DateTime date_created { get; set; }
            public DateTime date_studied { get; set; }
        }

        /**
         * This class should contain all information needed from the user to create a row in the database
         */
        public class PostSessionInfo
        {
            public string play_order { get; set; }
            public string learn_review { get; set; }
            public int max_cards { get; set; }
            public string platform { get; set; }
            public string device { get; set; }
            public string userEmail { get; set; }
            public int deckId { get; set; }
            public List<int> cards_played { get; set; }
        }

        // GET: api/Sessions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SessionInfo>>> GetSessions()
        {
            var query = from s in _context.Sessions
                        select new SessionInfo
                        {
                            id = s.DeckID,
                            play_order = s.PlayOrder.ToString(),
                            learn_review = s.LearnReview.ToString(),
                            max_cards = s.MaxCards,
                            platform = s.Platform.ToString(),
                            device = s.Device,
                            ownerId = s.UserId,
                            deckId = s.DeckID,
                            cards_played = s.CardsPlayed.Select(c => c.CardID).ToList(),
                            date_created = s.DateCreated,
                            date_studied = s.DateLastStudied
                        };
            return await query.ToListAsync();
        }

        // GET: api/Sessions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SessionInfo>> GetSession(int id)
        {
            var query = from s in _context.Sessions
                        where s.SessionID == id
                        select new SessionInfo
                        {
                            id = s.DeckID,
                            play_order = s.PlayOrder.ToString(),
                            learn_review = s.LearnReview.ToString(),
                            max_cards = s.MaxCards,
                            platform = s.Platform.ToString(),
                            device = s.Device,
                            ownerId = s.UserId,
                            deckId = s.DeckID,
                            cards_played = s.CardsPlayed.Select(c => c.CardID).ToList(),
                            date_created = s.DateCreated,
                            date_studied = s.DateLastStudied
                        };

            if (query.Count() == 0)
            {
                return NotFound();
            }

            return query.First();
        }

        /**
* Gets all sessions belonging to the given user provided by email
*/
        // GET: api/Sessions/User=johndoe@gmail.com
        [HttpGet("/Sessions/UserEmail={userEmail}")]
        public async Task<ActionResult<IEnumerable<SessionInfo>>> GetUserSessionsByEmail(string userEmail)
        {
            EchoUser user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                return BadRequest("User " + userEmail + " not found");
            }
            else
            {
                var query = from s in _context.Sessions
                            where s.UserId == user.Id
                            select new SessionInfo
                            {
                                id = s.DeckID,
                                play_order = s.PlayOrder.ToString(),
                                learn_review = s.LearnReview.ToString(),
                                max_cards = s.MaxCards,
                                platform = s.Platform.ToString(),
                                device = s.Device,
                                ownerId = s.UserId,
                                deckId = s.DeckID,
                                cards_played = s.CardsPlayed.Select(c => c.CardID).ToList(),
                                date_created = s.DateCreated,
                                date_studied = s.DateLastStudied
                            };

                return await query.ToListAsync();
            }
        }


        /**
         * Gets all sessions belonging to the given user
         */
        // GET: api/Sessions/User=johndoe@gmail.com
        [HttpGet("/Sessions/User={userId}")]
        public async Task<ActionResult<IEnumerable<SessionInfo>>> GetUserSessions(string userId)
        {
            var query = from s in _context.Sessions
                        where s.UserId == userId
                        select new SessionInfo
                        {
                            id = s.DeckID,
                            play_order = s.PlayOrder.ToString(),
                            learn_review = s.LearnReview.ToString(),
                            max_cards = s.MaxCards,
                            platform = s.Platform.ToString(),
                            device = s.Device,
                            ownerId = s.UserId,
                            deckId = s.DeckID,
                            cards_played = s.CardsPlayed.Select(c => c.CardID).ToList(),
                            date_created = s.DateCreated,
                            date_studied = s.DateLastStudied
                        };

            return await query.ToListAsync();
        }

        // PUT: api/Sessions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSession(int id, PostSessionInfo sessionInfo)
        {
            var sessionQuery = from s in _context.Sessions
                            where s.SessionID == id
                            select s;
            // Create the deck
            if (sessionQuery.Count() == 0)
            {
                // Create and populate a deck with the given info
                Session session = new Session();
                session.MaxCards = sessionInfo.max_cards;
                session.Device = sessionInfo.device;
                session.DeckID = sessionInfo.deckId;

                // Handle the enums with switch cases
                switch (sessionInfo.play_order.ToLower())
                {
                    case "random":
                        session.PlayOrder = PlayOrder.Random;
                        break;
                    case "sequential":
                        session.PlayOrder = PlayOrder.Sequential;
                        break;
                    default:
                        return BadRequest("Valid access parameters are Random and Sequential");
                }
                switch (sessionInfo.learn_review.ToLower())
                {
                    case "learn":
                        session.LearnReview = PlayType.Learn;
                        break;
                    case "review":
                        session.LearnReview = PlayType.Review;
                        break;
                    default:
                        return BadRequest("Valid learn_review parameters are learn and review");
                }
                switch (sessionInfo.platform.ToLower())
                {
                    case "web":
                        session.Platform = Platform.Web;
                        break;
                    default:
                        return BadRequest("Valid platforms are cureently web");
                }

                // Owner
                EchoUser user = await _userManager.FindByEmailAsync(sessionInfo.userEmail);
                if (user is null)
                {
                    return BadRequest("User " + sessionInfo.userEmail + " not found");
                }
                else
                {
                    session.UserId = user.Id;
                }


                // Assign the played cards
                session.CardsPlayed = new List<Card>();
                foreach (int cardId in sessionInfo.cards_played)
                {
                    // Grab the deck. Only possible for one or zero results since ids are unique.
                    var query = from c in _context.Cards
                                where c.CardID == cardId
                                select c;
                    if (query.Count() == 0)
                    {
                        return BadRequest("Card id " + cardId + " does not exist");
                    }
                    else
                    {
                        Card card = query.First();
                        session.CardsPlayed.Add(card);
                    }
                }

                // Dates
                session.DateCreated = DateTime.Now;
                session.DateLastStudied = DateTime.Now;

                // Save the session
                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();

                return CreatedAtAction("PutSession", new { message = "Session was successfully created.", id = session.SessionID });
            }
            // Update the deck
            else
            {
                Session session = sessionQuery.First();

                // Assign the owner
                EchoUser user = await _userManager.FindByEmailAsync(sessionInfo.userEmail);
                if (user is null)
                {
                    return BadRequest("User " + sessionInfo.userEmail + " not found");
                }
                else
                {
                    session.UserId = user.Id;
                }

                // Set max cards, device, deckid
                session.MaxCards = sessionInfo.max_cards;
                session.Device = sessionInfo.device;
                session.DeckID = sessionInfo.deckId;

                // Set the enums
                switch (sessionInfo.play_order.ToLower())
                {
                    case "random":
                        session.PlayOrder = PlayOrder.Random;
                        break;
                    case "sequential":
                        session.PlayOrder = PlayOrder.Sequential;
                        break;
                    default:
                        return BadRequest("Valid access parameters are Random and Sequential");
                }
                switch (sessionInfo.learn_review.ToLower())
                {
                    case "learn":
                        session.LearnReview = PlayType.Learn;
                        break;
                    case "review":
                        session.LearnReview = PlayType.Review;
                        break;
                    default:
                        return BadRequest("Valid learn_review parameters are learn and review");
                }
                switch (sessionInfo.platform.ToLower())
                {
                    case "web":
                        session.Platform = Platform.Web;
                        break;
                    default:
                        return BadRequest("Valid platforms are cureently web");
                }

                // Assign the played cards
                session.CardsPlayed = new List<Card>();
                foreach (int cardId in sessionInfo.cards_played)
                {
                    // Grab the deck. Only possible for one or zero results since ids are unique.
                    var query = from c in _context.Cards
                                where c.CardID == cardId
                                select c;
                    if (query.Count() == 0)
                    {
                        return BadRequest("Card id " + cardId + " does not exist");
                    }
                    else
                    {
                        Card card = query.First();
                        session.CardsPlayed.Add(card);
                    }
                }

                // Change the studied date
                session.DateLastStudied = DateTime.Now;

                // Mark the session
                _context.Entry(session).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Failed to update session");
                }

                return Ok(new { message = "Session was successfully updated.", id = session.SessionID });
            }
        }

        // POST: api/Sessions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("/Sessions")]
        public async Task<ActionResult<Session>> PostSession(PostSessionInfo sessionInfo)
        {
            // Create and populate a session with the given info
            Session session = new Session();
            session.MaxCards = sessionInfo.max_cards;
            session.Device = sessionInfo.device;
            session.DeckID = sessionInfo.deckId;

            // Handle the enums with switch cases
            switch (sessionInfo.play_order.ToLower())
            {
                case "random":
                    session.PlayOrder = PlayOrder.Random;
                    break;
                case "sequential":
                    session.PlayOrder = PlayOrder.Sequential;
                    break;
                default:
                    return BadRequest("Valid access parameters are Random and Sequential");
            }
            switch (sessionInfo.learn_review.ToLower())
            {
                case "learn":
                    session.LearnReview = PlayType.Learn;
                    break;
                case "review":
                    session.LearnReview = PlayType.Review;
                    break;
                default:
                    return BadRequest("Valid learn_review parameters are learn and review");
            }
            switch (sessionInfo.platform.ToLower())
            {
                case "web":
                    session.Platform = Platform.Web;
                    break;
                default:
                    return BadRequest("Valid platforms are cureently web");
            }

            // Owner
            EchoUser user = await _userManager.FindByEmailAsync(sessionInfo.userEmail);
            if (user is null)
            {
                return BadRequest("User " + sessionInfo.userEmail + " not found");
            }
            else
            {
                session.UserId = user.Id;
            }


            // Assign the played cards
            session.CardsPlayed = new List<Card>();
            foreach (int cardId in sessionInfo.cards_played)
            {
                // Grab the deck. Only possible for one or zero results since ids are unique.
                var query = from c in _context.Cards
                            where c.CardID == cardId
                            select c;
                if (query.Count() == 0)
                {
                    return BadRequest("Card id " + cardId + " does not exist");
                }
                else
                {
                    Card card = query.First();
                    session.CardsPlayed.Add(card);
                }
            }

            // Dates
            session.DateCreated = DateTime.Now;
            session.DateLastStudied = DateTime.Now;

            // Save the session
            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostSession", new { id = session.SessionID });
        }

        // DELETE: api/Sessions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSession(int id)
        {
            var session = await _context.Sessions.FindAsync(id);
            if (session == null)
            {
                return NotFound();
            }

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /*
* Updates the given card by id 
*/
        // PATCH: api/Sessions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchSession(int id, PostSessionInfo sessionInfo)
        {
            var sessionQuery = from s in _context.Sessions.Include(d => d.CardsPlayed)
                            where s.SessionID == id
                            select s;
            // Create the session
            if (sessionQuery.Count() == 0)
            {
                return BadRequest("Session " + id + " does not exist");
            }
            // Update the session
            else
            {
                Session session = sessionQuery.First();

                // Assign the owner
                EchoUser user = await _userManager.FindByEmailAsync(sessionInfo.userEmail);
                if (user is null)
                {
                    return BadRequest("User " + sessionInfo.userEmail + " not found");
                }
                else
                {
                    session.UserId = user.Id;
                }

                // Set max cards, device, deckid
                session.MaxCards = sessionInfo.max_cards;
                session.Device = sessionInfo.device;
                session.DeckID = sessionInfo.deckId;

                // Set the enums
                switch (sessionInfo.play_order.ToLower())
                {
                    case "random":
                        session.PlayOrder = PlayOrder.Random;
                        break;
                    case "sequential":
                        session.PlayOrder = PlayOrder.Sequential;
                        break;
                    default:
                        return BadRequest("Valid access parameters are Random and Sequential");
                }
                switch (sessionInfo.learn_review.ToLower())
                {
                    case "learn":
                        session.LearnReview = PlayType.Learn;
                        break;
                    case "review":
                        session.LearnReview = PlayType.Review;
                        break;
                    default:
                        return BadRequest("Valid learn_review parameters are learn and review");
                }
                switch (sessionInfo.platform.ToLower())
                {
                    case "web":
                        session.Platform = Platform.Web;
                        break;
                    default:
                        return BadRequest("Valid platforms are cureently web");
                }

                // Assign the played cards
                session.CardsPlayed = new List<Card>();
                foreach (int cardId in sessionInfo.cards_played)
                {
                    // Grab the deck. Only possible for one or zero results since ids are unique.
                    var query = from c in _context.Cards
                                where c.CardID == cardId
                                select c;
                    if (query.Count() == 0)
                    {
                        return BadRequest("Card id " + cardId + " does not exist");
                    }
                    else
                    {
                        Card card = query.First();
                        session.CardsPlayed.Add(card);
                    }
                }

                // Change the studied date
                session.DateLastStudied = DateTime.Now;

                // Mark the session
                _context.Entry(session).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Failed to update session");
                }

                return Ok(new { message = "Session was successfully updated.", id = session.SessionID });
            }
        }

        /*
        * Updates the study session by adding a card to cardsplayed and updates the date
        */
        // PATCH: api/Decks/Touch=1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("CardPlayed={cardId}&session={sessionId}")]
        public async Task<IActionResult> AddPlayedCard (int cardId, int sessionId)
        {
            var sessionQuery = from s in _context.Sessions.Include(s => s.CardsPlayed)
                            where s.SessionID == sessionId
                            select s;
            // Deck doesn't exist
            if (sessionQuery.Count() == 0)
            {
                return BadRequest("Session " + sessionId + " does not exist");
            }
            // Update the deck
            else
            {
                Session session = sessionQuery.First();

                // Add card to cards played
                var query = from c in _context.Cards
                            where c.CardID == cardId
                            select c;

                var card = query.FirstOrDefault();

                if (card == null)
                {
                    return NotFound();
                }

                session.CardsPlayed.Add(card);

                // Update touched date
                session.DateLastStudied = DateTime.Now;

                // Mark the card as modified
                _context.Entry(session).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Card was successfully added to played cards in session.", id = session.SessionID});
            }
        }

        /*
        * Updates the given session study date by id 
        */
        // PATCH: api/Sessions/Study=1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("Study={sessionId}")]
        public async Task<IActionResult> UpdateStudyDate (int sessionId)
        {
            var sessionQuery = from s in _context.Sessions.Include(s => s.CardsPlayed)
                               where s.SessionID == sessionId
                               select s;
            // Session doesn't exist
            if (sessionQuery.Count() == 0)
            {
                return BadRequest("Session " + sessionId + " does not exist");
            }
            // Update the session
            else
            {
                Session session = sessionQuery.First();

                // Update touched date
                session.DateLastStudied = DateTime.Now;

                // Mark the session as modified
                _context.Entry(session).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Study date successfully updated.", id = session.SessionID });
            }
        }

        /**
        * Deletes all decks associated with one user
        */
        // DELETE: api/Sessions/5
        [HttpDelete("/Sessions/DeleteUserSessions={userId}")]
        public async Task<IActionResult> DeleteUserSessions(string userId)
        {
            var query = from s in _context.Sessions
                        where s.UserId == userId
                        select s;

            List<Session> userSessions = await query.ToListAsync();
            foreach (Session session in userSessions)
            {
                _context.Sessions.Remove(session);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        /**
         * Deletes all decks associated with one user
         */
        // DELETE: api/Decks/DeleteUserDecksByEmail=johnDoe@gmail.com
        [HttpDelete("/Sessions/DeleteUserSessionsByEmail={userEmail}")]
        public async Task<IActionResult> DeleteUserDecksByEmail(string userEmail)
        {
            EchoUser user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
            {
                return BadRequest("User " + userEmail + " not found");
            }
            else
            {
                var query = from s in _context.Sessions
                            where s.UserId == user.Id
                            select s;

                List<Session> userSessions = await query.ToListAsync();
                foreach (Session session in userSessions)
                {
                    _context.Sessions.Remove(session);
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.SessionID == id);
        }
    }
}
