using static echoStudy_webAPI.Controllers.DecksController;
using System.Collections.Generic;

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
         * All of the user's public decks
         */
        public List<DeckInfo> Decks { get; set; }
    }
}