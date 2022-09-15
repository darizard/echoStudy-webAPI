using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static echoStudy_webAPI.Tests.Models.Models;

namespace echoStudy_webAPI.Tests
{
    internal static class HelperFunctions
    {

        /**
        * Gets John's token, should not be empty
         */
        public static async Task GrabJohnToken(HttpClient client)
        {
            // The user
            UserInfo userDetails = new UserInfo();
            userDetails.username = "JohnDoe@gmail.com";
            userDetails.password = "123ABC!@#def";

            // Get the token from the server
            HttpContent signInContent = new StringContent(JsonConvert.SerializeObject(userDetails), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("authenticate", signInContent);

            // Parse and set the authorization header in the client
            if (response.IsSuccessStatusCode)
            {
                authResponse token = JsonConvert.DeserializeObject<authResponse>(await response.Content.ReadAsStringAsync());

                client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token.Token);
                return;
            }

            throw new Exception("Couldn't obtain John's JWT Token");
        }
        /**
        * Grabs the user ID from the JWT token
        */
        public static async Task<string> GrabUserId(HttpClient client)
        {
            // TEST 1: Get a card that belongs to John
            HttpResponseMessage response = await client.GetAsync("Cards/1");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                CardInfo card = JsonConvert.DeserializeObject<CardInfo>(contents);
                return card.ownerId;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
