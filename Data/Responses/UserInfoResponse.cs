using System;

namespace echoStudy_webAPI.Data.Responses
{
    // All information on one user
    public class UserInfoResponse
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Id { get; set; }
        public DateTime DateCreated { get; set; }
    }
}