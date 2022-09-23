using echoStudy_webAPI.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static echoStudy_webAPI.Controllers.DecksController;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using echoStudy_webAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using echoStudy_webAPI.Data;

namespace echoStudy_webAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class PublicController : EchoUserControllerBase
    {
        EchoStudyDB _context;

        public PublicController(UserManager<EchoUser> um,
                                EchoStudyDB context) : base(um)
        {
            _context = context;
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IQueryable<DeckInfo>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<DeckInfo>>> GetDecks()
        {
            // Query the DB for the deck objects
            var query = from d in _context.Decks.Include(d => d.Cards)
                        where d.Access == Access.Public
                        select d;
            var decks = await query.ToListAsync();

            // Build the deck info objects
            List<DeckInfo> deckInfo = new List<DeckInfo>();
            foreach (Deck d in decks)
            {
                deckInfo.Add(new DeckInfo
                {
                    id = d.DeckID,
                    title = d.Title,
                    description = d.Description,
                    access = d.Access.ToString(),
                    default_flang = d.DefaultFrontLang.ToString(),
                    default_blang = d.DefaultBackLang.ToString(),
                    cards = d.Cards.Select(c => c.CardID).ToList(),
                    ownerId = d.UserId,
                    studyPercent = calculateStudyPercent(d),
                    date_created = d.DateCreated,
                    date_touched = d.DateTouched,
                    date_updated = d.DateUpdated
                });
            }

            return Ok(deckInfo);
        }

        /**
         * Calculates the study percent for a given deck
         * NOTE: If the deck is obtained from a query, make sure to include the entire card objects
         */
        private double calculateStudyPercent(Deck deck)
        {
            double studyCount = 0.0;
            double cardCount = 0.0;
            foreach (Card c in deck.Cards)
            {
                cardCount++;

                // Conditions that must be true for a card to be "studied"
                // 1. Card was touched at least once
                // 2. Card was touched at the same time or after dateUpdated.
                if (c.DateTouched != c.DateCreated && c.DateTouched >= c.DateUpdated)
                {
                    studyCount++;
                }
            }

            return Math.Round(studyCount / cardCount, 4);
        }
    }
}
