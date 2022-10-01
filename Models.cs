using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Tests.Models
{
    internal static class Models
    {
        public class authResponse
        {
            public string RefreshToken { get; set; }
            public string Token { get; set; }
        }


        /**
         * User Info
         */
        public class UserInfo
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        /**
         * Response from API
         */
        public class CreatedResponse
        {
            public int id { get; set; }
            public DateTime dateCreated { get; set; }
        }

        /**
         * Response from API
         */
        public class UpdatedResponse
        {
            public int id { get; set; }
            public DateTime DateUpdated { get; set; }
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
        * This class should contain all information needed in a GET request
        */
        public class DeckInfo
        {
            public int id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string access { get; set; }
            public string default_flang { get; set; }
            public string default_blang { get; set; }
            public string ownerId { get; set; }
            public List<int> cards { get; set; }
            public DateTime date_created { get; set; }
            public DateTime date_updated { get; set; }
            public DateTime date_touched { get; set; }
        }

        /**
         * This class should contain all information needed from the user to create a row in the database
         */
        public class PostDeckInfo
        {
            public string title { get; set; }
            public string description { get; set; }
            public string access { get; set; }
            public string default_flang { get; set; }
            public string default_blang { get; set; }
            public string userId { get; set; }
            public List<int> cardIds { get; set; }
        }

        // information used in creating a new user
        public class RegisterUserRequest
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string PhoneNumber { get; set; }
        }

        // Information required for changing a user's password
        public class ChangePasswordRequest
        {
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        public class UserInfoResponse
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
        }
        
        public class StudyRequest
        {
            public int id { get; set; }
            public int score { get; set; }
        }
    }
}
