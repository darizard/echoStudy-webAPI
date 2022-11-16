using echoStudy_webAPI.Areas.Identity.Data;
using echoStudy_webAPI.Data.Requests;
using echoStudy_webAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StudyActivityController : EchoUserControllerBase
    {
        private readonly EchoStudyDB _context;

        public StudyActivityController(EchoStudyDB context, UserManager<EchoUser> um) : base(um)
        {
            _context = context;
        }

        // GET /studyactivity
        /// <summary>
        /// Returns an object representing the currently authenticated user's study activity
        /// </summary>
        /// <response code="200">Returns a list of all usernames</response>
        /// <response code="401">A valid, non-expired token was not received in the Authorization header</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [HttpGet]
        public async Task<IActionResult> GetActivity([FromQuery] ActivityRetrievalRequest request)
        {
            DateTime? start = request.StartDate is null ? DateTime.MinValue : request.StartDate;
            DateTime? end = request.EndDate is null ? DateTime.MaxValue : request.EndDate;

            var query = from sa in _context.StudyActivity
                        where sa.DateStudied.Date <= end && sa.DateStudied.Date >= start && sa.UserId == _user.Id
                        select new
                        {
                            sa.DeckId,
                            sa.DateStudied
                        };

            var records = await query.ToListAsync();

            // build a set of keys (dates studied) with related lists of deckIds
            Dictionary<DateTime, List<int?>> resultsDict = new();
            foreach(var record in records)
            {
                if(!resultsDict.ContainsKey(record.DateStudied.Date))
                {
                    resultsDict[record.DateStudied.Date] = new List<int?>();
                }
                resultsDict[record.DateStudied.Date].Add(record.DeckId);
            }

            // configure the dict into a list of objects where date is a key and decks is its related list of deck ids
            List<object> rtnval = new();
            foreach(var dateEntry in resultsDict)
            {
                rtnval.Add(new
                {
                    date = dateEntry.Key,
                    decks = dateEntry.Value
                });
            }

            return Ok(rtnval);
        }
    }
}
