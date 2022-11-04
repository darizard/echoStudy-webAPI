using System;

namespace echoStudy_webAPI.Data.Requests
{
    public class ActivityRetrievalRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
