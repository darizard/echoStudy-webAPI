using Microsoft.AspNetCore.Mvc;
using System;

namespace echoStudy_webAPI.Data.Requests
{
    public class ActivityRetrievalRequest
    {
        [FromQuery(Name = "startDate")]
        public DateTime? StartDate { get; set; }

        [FromQuery(Name = "endDate")]
        public DateTime? EndDate { get; set; }
    }
}
