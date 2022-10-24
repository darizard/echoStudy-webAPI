using static echoStudy_webAPI.Controllers.DecksController;
using System.Collections.Generic;
using System;

namespace echoStudy_webAPI.Data.Responses
{
    /**
     * Information of a user to be displayed on a public profile
     */
    public class UserInfoPublicResponse
    {
        /**
         * User's public display name
        */
        public string Username { get; set; }
        /**
        * URL to the user's profile picture
        */
        public string ProfilePicture { get; set; }
        /**
         * Date of the user creating their account
         */
        public DateTime DateCreated { get; set; }
        /**
         * All of the user's public decks
         */
        public List<DeckInfo> Decks { get; set; }

    }
}