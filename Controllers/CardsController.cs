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
using Microsoft.AspNetCore.Authorization;
using static echoStudy_webAPI.Controllers.CardsController;

namespace echoStudy_webAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CardsController : EchoUserControllerBase
    {
        private readonly EchoStudyDB _context;

        public CardsController(EchoStudyDB context, UserManager<EchoUser> um)
            : base(um)
        {
            _context = context;
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
            public int deckId { get; set; }
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
            public int? deckId { get; set; }
        }

        /**
        * Info required to "study" a card
        */
        public class PostStudyInfo
        {
            public int id { get; set; }
            public int? score { get; set; }
        }

        /**
        * Define response type for studying a card
        */
        public class PostStudyResponse
        {
            public List<int> ids { get; set; }
            public DateTime dateStudied { get; set; }
        }

        /**
        * Define response type for batch Card creation
        */
        public class CardCreationResponse
        {
            public List<int> ids { get; set; }
            public DateTime dateCreated { get; set; }
        }

        /**
         * Define response type for Card update
         */
        public class CardUpdateResponse
        {
            public int id { get; set; }
            public DateTime dateUpdated { get; set; }
        }

        // GET: /Cards
        /// <summary>
        /// Retrieves all Card objects owned by the authenticated user or all Card
        /// objects associated with a specific Deck owned by the authenticated user
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="deckId">The ID of the related deck</param>"
        /// <response code="200">Returns the queried Card objects</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to access the specified deck</response>
        /// <response code="404">Object not found with the provided userId, userEmail, or deckId</response>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<CardInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CardInfo>>> GetCards(int? deckId)
        {
            List<CardInfo> cards;

            if (deckId is not null)
            {
                var querydeck = from d in _context.Decks
                                where d.DeckID == deckId
                                select d;

                List<Deck> decks = await querydeck.ToListAsync();

                if (decks.Count == 0)
                {
                    return NotFound();
                }

                if (decks.First().UserId != _user.Id)
                {
                    return Forbid();
                }

                var querycards = from c in _context.Cards
                                 where c.DeckID == deckId
                                 select new CardInfo
                                 {
                                     id = c.CardID,
                                     ftext = c.FrontText,
                                     btext = c.BackText,
                                     faud = AmazonUploader.getPresignedUrl(c.FrontText, c.FrontLang),
                                     baud = AmazonUploader.getPresignedUrl(c.BackText, c.BackLang),
                                     flang = c.FrontLang.ToString(),
                                     blang = c.BackLang.ToString(),
                                     deckId = c.DeckID,
                                     score = c.Score,
                                     ownerId = c.UserId,
                                     date_created = c.DateCreated,
                                     date_updated = c.DateUpdated,
                                     date_touched = c.DateTouched
                                 };

                cards = await querycards.ToListAsync();
                return Ok(cards);
            }

            var queryuser = from c in _context.Cards
                            where c.UserId == _user.Id
                            select new CardInfo
                            {
                                id = c.CardID,
                                ftext = c.FrontText,
                                btext = c.BackText,
                                faud = AmazonUploader.getPresignedUrl(c.FrontText, c.FrontLang),
                                baud = AmazonUploader.getPresignedUrl(c.BackText, c.BackLang),
                                flang = c.FrontLang.ToString(),
                                blang = c.BackLang.ToString(),
                                deckId = c.DeckID,
                                score = c.Score,
                                ownerId = c.UserId,
                                date_created = c.DateCreated,
                                date_updated = c.DateUpdated,
                                date_touched = c.DateTouched
                            };
            return Ok(await queryuser.ToListAsync());
        }

        // GET: /Cards/{id}
        /// <summary>
        /// Retrieves one Card specified by Id
        /// </summary>
        /// <remarks>
        /// <param name="id">Card ID</param>
        /// </remarks>
        /// <response code="200">Returns the queried Card object</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to access the specified card</response>
        /// <response code="404">Object not found with the cardId</response>
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CardInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(CardInfo), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CardInfo>> GetCard(int id)
        {
            var query = from c in _context.Cards
                        where c.CardID == id
                        select new CardInfo
                        {
                            id = c.CardID,
                            ftext = c.FrontText,
                            btext = c.BackText,
                            faud = AmazonUploader.getPresignedUrl(c.FrontText, c.FrontLang),
                            baud = AmazonUploader.getPresignedUrl(c.BackText, c.BackLang),
                            flang = c.FrontLang.ToString(),
                            blang = c.BackLang.ToString(),
                            deckId = c.DeckID,
                            score = c.Score,
                            ownerId = c.UserId,
                            date_created = c.DateCreated,
                            date_updated = c.DateUpdated,
                            date_touched = c.DateTouched
                        };

            CardInfo card = await query.FirstOrDefaultAsync();

            if (card is null)
            {
                return NotFound("CardId " + id + " was not found");
            }

            if (card.ownerId != _user.Id)
            {
                return Forbid();
            }

            return Ok(await query.FirstAsync());
        }

        // Post: /Cards/{id}
        /// <summary>
        /// Edits an existing Card
        /// </summary>
        /// <remarks></remarks>
        /// <param name="id">ID of the Card to edit</param>
        /// <param name="cardInfo">
        /// Optional: frontText, backText, frontLang, backLang, deckId
        /// </param>
        /// <response code="200">Returns the Id and DateUpdated of the edited Card</response>
        /// <response code="400">Invalid input or input type</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to access the specified card</response>
        /// <response code="404">Object at the cardId provided was not found</response>
        /// <response code="500">Database failed to complete card update despite valid request</response>
        [HttpPost("{id}")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(typeof(CardUpdateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostCardEdit(int id, [FromBody] PostCardInfo cardInfo)
        {
            // Ensure the card exists and that the owner made the request
            Card card = await _context.Cards.FindAsync(id);
            if (card is null)
            {
                return NotFound("Card id " + id + " not found");
            }
            if (card.UserId != _user.Id)
            {
                return Forbid();
            }

            //-------Update according to incoming info

            // if the card is changing decks, we need to update both decks
            int? olddeckid = null;
            if (cardInfo.deckId is not null)
            {
                olddeckid = card.DeckID;
                card.DeckID = (int)cardInfo.deckId;

                if (olddeckid != cardInfo.deckId)
                {
                    // Update/validate the new deck
                    var newQuery = from d in _context.Decks.Include(d => d.Cards)
                                   where d.DeckID == cardInfo.deckId
                                   select d;
                    Deck newDeck = await newQuery.FirstOrDefaultAsync();
                    // Ensure the new deck is valid and then add the card and change the updated date
                    if (newDeck is null)
                    {
                        return NotFound("Deck id " + cardInfo.deckId + " not found");
                    }
                    if (newDeck.UserId != _user.Id)
                    {
                        return Forbid();
                    }
                    newDeck.Cards.Add(card);
                    newDeck.DateUpdated = DateTime.Now;

                    // Update the old deck if it exists
                    var oldQuery = from d in _context.Decks.Include(d => d.Cards)
                                   where d.DeckID == olddeckid
                                   select d;
                    Deck oldDeck = await oldQuery.FirstOrDefaultAsync();
                    if (oldDeck is not null)
                    {
                        oldDeck.Cards.Remove(card);
                        oldDeck.DateUpdated = DateTime.Now;
                    }
                    _context.Decks.Update(oldDeck);
                    _context.Decks.Update(newDeck);
                }
                else
                {
                    Deck deck = await _context.Decks.FindAsync(olddeckid);
                    deck.DateUpdated = DateTime.Now;
                    _context.Decks.Update(deck);
                }
            }

            if (cardInfo.frontText is not null)
            {
                card.FrontText = cardInfo.frontText;
            }
            if (cardInfo.backText is not null)
            {
                card.BackText = cardInfo.backText;
            }
            if (cardInfo.frontLang is not null)
            {
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
            }
            if (cardInfo.backLang is not null)
            {
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
            }

            // Dates
            card.DateUpdated = DateTime.Now;

            // if we changed the text or language of the front or back of the card, make a call to Polly
            if (cardInfo.frontLang is not null || cardInfo.frontText is not null)
            {
                card.FrontAudio = AmazonPolly.createTextToSpeechAudio(card.FrontText, card.FrontLang);
            }
            if (cardInfo.backLang is not null || cardInfo.backText is not null)
            {
                card.BackAudio = AmazonPolly.createTextToSpeechAudio(card.BackText, card.BackLang);
            }

            _context.Cards.Update(card);

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

            return Ok(new CardUpdateResponse
            {
                id = card.CardID,
                dateUpdated = card.DateUpdated
            });
        }

        // POST: /Cards
        /// <summary>
        /// Creates cards for the currently authenticated user through the provided list
        /// </summary>
        /// <param name="cards">
        /// List of cards to create.
        /// Required: frontText, backText, frontLang, backLang, deckId
        /// </param>
        /// <remarks></remarks>
        /// <response code="201">Returns the id and creation date of the created Card</response>
        /// <response code="400">Invalid input or input type</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to add a card to the specified deck</response>
        /// <response code="404">Deck at deckId provided was not found</response>
        /// <response code="500">Database failed to complete card creation despite valid request</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CardCreationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostCardCreate(List<PostCardInfo> cards)
        {
            // Create each card provided and log them as they are created
            List<Card> createdCards = new List<Card>();
            foreach (PostCardInfo cardInfo in cards)
            {
                // Required fields
                if (cardInfo.frontText is null)
                {
                    return BadRequest("frontText is required at index " + createdCards.Count);
                }
                if (cardInfo.backText is null)
                {
                    return BadRequest("backText is required at index " + createdCards.Count);
                }
                if (cardInfo.frontLang is null)
                {
                    return BadRequest("frontLang is required at index " + createdCards.Count);
                }
                if (cardInfo.backLang is null)
                {
                    return BadRequest("backLang is required at index " + createdCards.Count);
                }
                if (cardInfo.deckId is null)
                {
                    return BadRequest("deckId is required at index " + createdCards.Count);
                }

                // Get the related deck
                var deckQuery = from d in _context.Decks
                                where d.DeckID == cardInfo.deckId
                                select d;

                Deck deck = await deckQuery.FirstOrDefaultAsync();
                if (deck is null)
                {
                    return NotFound("Deck ID " + cardInfo.deckId + " not found at index " + createdCards.Count);
                }
                if (deck.UserId != _user.Id)
                {
                    return Forbid();
                }

                // Create a card and assign it all of the provided data
                Card card = new Card();
                card.UserId = _user.Id;
                card.FrontText = cardInfo.frontText;
                card.BackText = cardInfo.backText;
                card.DeckID = (int)cardInfo.deckId;
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
                        return BadRequest("Current supported languages are English, Spanish, Japanese, and German. Card at index " + createdCards.Count + " has a language not listed");
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
                        return BadRequest("Current supported languages are English, Spanish, Japanese, and German. Card at index " + createdCards.Count + " has a language not listed");
                }

                // Assign it dates and a score of 0
                card.DateCreated = DateTime.Now;
                card.DateTouched = DateTime.Now;
                card.DateUpdated = DateTime.Now;
                card.Score = 0;
                card.DeckPosition = "";

                // Make Polly calls
                card.FrontAudio = AmazonPolly.createTextToSpeechAudio(card.FrontText, card.FrontLang);
                card.BackAudio = AmazonPolly.createTextToSpeechAudio(card.BackText, card.BackLang);

                // update related deck's DateUpdated
                card.Deck = deck;
                deck.DateUpdated = card.DateUpdated;
                _context.Decks.Update(deck);

                // Ready to add
                _context.Cards.Add(card);
                createdCards.Add(card);
            }

            // Save all the added cards
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

            // Now that save changes was called the IDs are generated
            List<int> ids = new List<int>();
            foreach (Card c in createdCards)
            {
                ids.Add(c.CardID);
            }

            // Indicate success
            return CreatedAtAction("PostCardCreate", new CardCreationResponse
            {
                ids = ids,
                dateCreated = DateTime.Now
            });
        }

        // Post: /Cards/Study
        /// <summary>
        /// Studies the cards related to the provided IDs
        /// </summary>
        /// <remarks>Score is optional</remarks>
        /// <param name="studyInfos">Info required for studying a card. Score may be null/omitted to keep the same score. </param>
        /// <response code="200">Returns the id of the card and study date of the card</response>
        /// <response code="400">Invalid input or input type</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to access the specified card</response>
        /// <response code="404">Object at the cardId provided was not found</response>
        /// <response code="500">Database failed to complete card update despite valid request</response>
        [HttpPost("Study")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(typeof(PostStudyResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequestResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostCardStudy(List<PostStudyInfo> studyInfos)
        {
            List<int> ids = new List<int>();
            foreach(PostStudyInfo studyInfo in studyInfos)
            {
                // Ensure the card exists and that the owner made the request
                Card card = await _context.Cards.FindAsync(studyInfo.id);
                if (card is null)
                {
                    return NotFound("Card id " + studyInfo.id + " not found");
                }
                if (card.UserId != _user.Id)
                {
                    return Forbid();
                }

                // Ensure that the deck exists and that the owner made the request
                Deck deck = await _context.Decks.FindAsync(card.DeckID);
                if (deck is null)
                {
                    return NotFound("Deck id " + card.DeckID + " not found");
                }
                if (deck.UserId != _user.Id)
                {
                    return Forbid();
                }

                // Set the score if it was provided
                if (studyInfo.score is not null)
                {
                    card.Score = (int)studyInfo.score;
                }

                // Update touched date on the card and deck
                card.DateTouched = DateTime.Now;
                deck.DateTouched = DateTime.Now;

                // Update the card and deck
                _context.Cards.Update(card);
                _context.Decks.Update(deck);
                ids.Add(card.CardID);
            }

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

            // Indicate success
            return Ok(new PostStudyResponse
            {
                ids = ids,
                dateStudied = DateTime.Now
            });
        }

        // DELETE: /Cards/Delete
        /// <summary>
        /// Deletes the card related to the provided ID
        /// </summary>
        /// <param name="id">ID of the card to be deleted</param>
        /// <response code="204"></response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        /// <response code="403">The current user is not authorized to access the specified card</response>
        /// <response code="404">Object at cardId was not found</response>
        /// <response code="500">Database failed to complete card deletion despite valid request</response>
        [HttpPost("Delete")]
        [Produces("text/plain", "application/json")]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(StatusCodeResult), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCard(int id)
        {
            // Delete the card
            Card card = await _context.Cards.FindAsync(id);

            if (card == null)
            {
                return NotFound();
            }
            if (card.UserId != _user.Id)
            {
                return Forbid();
            }

            _context.Cards.Remove(card);

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

        /**
         * Deletes all cards associated with one user
         */
        /*
        // DELETE: api/Cards/5
        [HttpDelete("/Cards/DeleteUserCards={userId}")]
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
        */

        /**
 * Deletes all cards associated with one user
 */
        /*
        // DELETE: api/Cards/5
        [HttpDelete("/Cards/DeleteUserCardsByEmail={userEmail}")]
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
        */

        /**
         * Deletes all cards associated with a deck
         */
        /*
        [HttpDelete("/Cards/DeleteDeckCards={deckId}")]
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
        */

        /*
        private bool CardExists(int id)
        {
            return _context.Cards.Any(e => e.CardID == id);
        }
        */
    }
}
