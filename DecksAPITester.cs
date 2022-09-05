using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static echoStudy_webAPI.Tests.CardsAPITester;

namespace echoStudy_webAPI.Tests
{
    [TestClass]
    public class DeckAPITester
    {
        public HttpClient client;

        // Determines whether localhost or api.echostudy.com is tested
        public bool testDevelopment = true;

        // Token for the user to be tested
        public bool userTokenSet;

        // The user's id
        public string userId;

        public string johnDoeId;

        public DeckAPITester()
        {
            if (testDevelopment)
            {
                client = new HttpClient();
                client.BaseAddress = new Uri("https://localhost:44397/");
            }
            else
            {
                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };

                client = new HttpClient(handler);
                client.BaseAddress = new Uri("http://api.echostudy.com/");
            }

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /**
        * Tests the GET Decks endpoint 
        */
        [TestMethod]
        public async Task GetDecksTest()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
            }

            HttpResponseMessage response = await client.GetAsync("Decks");
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<DeckInfo> decks = JsonConvert.DeserializeObject<List<DeckInfo>>(contents);
                Assert.AreNotEqual(0, decks.Count);
            }
            else
            {
                Assert.Fail("Failed to retrieve John's decks");
            }
        }

        /**
        * Tests the GET Decks/Public endpoint 
        */
        [TestMethod]
        public async Task GetDecksPublicTest()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
            }

            HttpResponseMessage response = await client.GetAsync("Decks/Public");
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<DeckInfo> decks = JsonConvert.DeserializeObject<List<DeckInfo>>(contents);
                Assert.AreNotEqual(0, decks.Count);
            }
            else
            {
                Assert.Fail("Failed to retrieve public decks");
            }
        }

        /**
         * Tests the GET Decks/{id} endpoint with all possible types requests
         */
        [TestMethod]
        public async Task GetDecksIdTests()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
            }

            // TEST 1: Get a deck that John owns
            HttpResponseMessage response = await client.GetAsync("Decks/1");
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                DeckInfo deck = JsonConvert.DeserializeObject<DeckInfo>(contents);
            }
            else
            {
                Assert.Fail("TEST 1: Request failed to get valid deck");
            }

            // TEST 2: Get a deck that John doesn't own
            response = await client.GetAsync("Decks/5");
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 2: Request succeeded to get a deck that John doesn't own");
            }
            else
            {
                if(response.StatusCode != System.Net.HttpStatusCode.Forbidden)
                {
                    Assert.Fail("TEST 2: Status code was not Forbidden when it should be");
                }
            }

            // TEST 3: Get a deck that doesn't exist
            response = await client.GetAsync("Decks/-1");
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 2: Request succeeded to get a deck that doesn't exist");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("TEST 2: Status code was not NotFound when it should be");
                }
            }
        }

        /**
        * Tests the POST Deck endpoint with all possible types requests
        */
        [TestMethod]
        public async Task PostDecksTests()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
                await grabUserId();
            }

            // TEST 1: Ensure that decks with empty fields won't be created successfully
            PostDeckInfo deckInfo = new PostDeckInfo();
            // no title
            populateDeck(deckInfo);
            deckInfo.title = null;
            HttpResponseMessage response = await client.PostAsync("Decks", createContent(deckInfo));
            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("TEST 1: Status code was not BadRequest when it should have been");
                } 
            }
            else
            {
                Assert.Fail("TEST 1: Request succeeded with null title");
            }
            // no description
            populateDeck(deckInfo);
            deckInfo.description = null;
            response = await client.PostAsync("Decks", createContent(deckInfo));
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("TEST 1: Status code was not BadRequest when it should have been");
                }
            }
            else
            {
                Assert.Fail("TEST 1: Request succeeded with null description");
            }
            // no backlang
            populateDeck(deckInfo);
            deckInfo.default_blang = null;
            response = await client.PostAsync("Decks", createContent(deckInfo));
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("TEST 1: Status code was not BadRequest when it should have been");
                }
            }
            else
            {
                Assert.Fail("TEST 1: Request succeeded with null default_blang");
            }
            // no frontlang
            populateDeck(deckInfo);
            deckInfo.default_flang = null;
            response = await client.PostAsync("Decks", createContent(deckInfo));
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("TEST 1: Status code was not BadRequest when it should have been");
                }
            }
            else
            {
                Assert.Fail("TEST 1: Request succeeded with null default_flang");
            }

            // TEST 2: Ensure that a deck can be created successfully for John
            deckInfo = new PostDeckInfo();
            populateDeck(deckInfo);
            response = await client.PostAsync("Decks", createContent(deckInfo));
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                CreatedResponse createdResponse = JsonConvert.DeserializeObject<CreatedResponse>(contents);
                response = await client.GetAsync("Decks/" + createdResponse.id);

                if (response.IsSuccessStatusCode)
                {
                    contents = await response.Content.ReadAsStringAsync();
                    DeckInfo deck = JsonConvert.DeserializeObject<DeckInfo>(contents);

                    // Ensure everything is right
                    Assert.AreEqual(userId, deck.ownerId);
                    Assert.AreEqual(deckInfo.title, deck.title);
                    Assert.AreEqual(deckInfo.description, deck.description);
                    Assert.AreEqual(deckInfo.default_flang, deck.default_flang);
                    Assert.AreEqual(deckInfo.default_blang, deck.default_blang);

                    // Delete the deck
                    response = await client.PostAsync("Decks/Delete/" + createdResponse.id, null);
                    if (!response.IsSuccessStatusCode)
                    {
                        Assert.Fail("TEST 2: Request failed to delete created deck");
                    }
                }
                else
                {
                    Assert.Fail("TEST 2: Request failed to retrieve the created deck");
                }
            }
            else
            {
                Assert.Fail("TEST 2: Request failed to create deck");
            }
        }

        /**
        * Tests the POST Decks endpoint with all possible types requests
        */
        [TestMethod]
        public async Task PostEditDecksTests()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
                await grabUserId();
            }

            // TEST 1: Ensure that a valid deck can be edited
            // First, store the old info
            HttpResponseMessage response = await client.GetAsync("Decks/1");
            DeckInfo oldDeck = new DeckInfo();
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                oldDeck = JsonConvert.DeserializeObject<DeckInfo>(contents);
            }
            else
            {
                Assert.Fail("TEST 1: Request failed to grab the deck to be edited");
            }

            // Deck
            PostDeckInfo deckInfo = new PostDeckInfo();
            populateDeck(deckInfo);
            response = await client.PostAsync("Decks/1", createContent(deckInfo));

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                UpdatedResponse updatedResponse = JsonConvert.DeserializeObject<UpdatedResponse>(contents);
                response = await client.GetAsync("Decks/" + updatedResponse.id);

                if (response.IsSuccessStatusCode)
                {
                    contents = await response.Content.ReadAsStringAsync();
                    DeckInfo deck = JsonConvert.DeserializeObject<DeckInfo>(contents);

                    // Ensure everything is right
                    Assert.AreEqual(userId, deck.ownerId);
                    Assert.AreEqual(deckInfo.title, deck.title);
                    Assert.AreEqual(deckInfo.description, deck.description);
                    Assert.AreEqual(deckInfo.default_flang, deck.default_flang);
                    Assert.AreEqual(deckInfo.default_blang, deck.default_blang);

                    // Change it all back
                    deckInfo.userId = oldDeck.ownerId;
                    deckInfo.title = oldDeck.title;
                    deckInfo.description = oldDeck.description;
                    deckInfo.default_blang = oldDeck.default_blang;
                    deckInfo.default_flang = oldDeck.default_flang;
                    response = await client.PostAsync("Decks/1", createContent(deckInfo));
                    if (!response.IsSuccessStatusCode)
                    {
                        Assert.Fail("TEST 1: Request failed to restore the old deck");
                    }
                }
                else
                {
                    Assert.Fail("TEST 1: Request failed to retrieve updated deck");
                }
            }
            else
            {
                Assert.Fail("TEST 1: Request failed to edit deck");
            }

            // TEST 2: Ensure that a deck that John doesn't own can't be edited
            deckInfo = new PostDeckInfo();
            populateDeck(deckInfo);
            response = await client.PostAsync("Decks/6", createContent(deckInfo));

            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode != HttpStatusCode.Forbidden)
                {
                    Assert.Fail("TEST 2: Status code should be Forbidden but wasn't");
                }
            }
            else
            {
                Assert.Fail("TEST 2: Request succeeded for a deck John doesn't own");
            }

        }

        /**
        * Tests the POST /Decks/Delete/{id} endpoint with all possible types of requests
        */
        [TestMethod]
        public async Task PostDeckDeleteIdTest()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
            }

            // TEST 1: Ensure request for non-existent deck fails
            HttpResponseMessage response = await client.PostAsync("Decks/Delete/-1", null);
            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode != HttpStatusCode.NotFound)
                {
                    Assert.Fail("TEST 1: Status code was not NotFound");
                }  
            }
            else
            {
                Assert.Fail("TEST 1: Request succeeded for non-existent deck");
            }

            // TEST 2: Ensure request for a deck John doesn't own fails
            response = await client.PostAsync("Decks/Delete/6", null);
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode != HttpStatusCode.Forbidden)
                {
                    Assert.Fail("TEST 2: Status code was not Forbidden");
                }
            }
            else
            {
                Assert.Fail("TEST 2: Request succeeded for a deck John doesn't own");
            }

            // TEST 3: Ensure that a deletion request for a valid deck succeeds
            PostDeckInfo deckInfo = new PostDeckInfo();
            populateDeck(deckInfo);
            response = await client.PostAsync("Decks", createContent(deckInfo));

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                CreatedResponse createdResponse = JsonConvert.DeserializeObject<CreatedResponse>(contents);
                response = await client.PostAsync("Decks/Delete/" + createdResponse.id, null);
                if (!response.IsSuccessStatusCode)
                {
                    Assert.Fail("TEST 3: Request failed to delete the created deck");
                }
            }
            else
            {
                Assert.Fail("TEST 3: Request failed to create the deck to be deleted");
            }
        }

        /**
        * Tests the POST /Decks/Delete endpoint with ONLY a bad request
        * The query will delete all decks and that is simply too much to restore, especially on the live site.
        */
        [TestMethod]
        public async Task PostDeckDeleteTest()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
            }

            // TEST 1: Ensure request for non-existent deck fails
            HttpResponseMessage response = await client.PostAsync("Decks/Delete", new StringContent(JsonConvert.SerializeObject("123"), Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode != HttpStatusCode.Forbidden)
                {
                    Assert.Fail("TEST 1: Status code was not Forbidden");
                }
            }
            else
            {
                Assert.Fail("TEST 1: Request succeeded for a user that isn't John");
            }

            // TEST 2: Delete all of John's decks and restore them
            // maybe 
        }


        /**
        * Gets John's token, should not be empty
        f */
        public async Task GrabJohnToken()
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
                client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync()));
                userTokenSet = true;
                return;
            }

            throw new Exception("Unable to retrieve John's token");
        }

        /**
        * Creates an HttpContent object with the given deck
        */
        private HttpContent createContent(PostDeckInfo deck)
        {
            // Serialize our concrete class into a JSON String
            var stringPayload = JsonConvert.SerializeObject(deck);

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            return new StringContent(stringPayload, Encoding.UTF8, "application/json");
        }

        /**
         * Populates a PostDeckInfo with valid info
         */
        private void populateDeck(PostDeckInfo deckInfo)
        {
            deckInfo.title = "Fake deck";
            deckInfo.description = "Fake deck used for testing purposes";
            deckInfo.default_flang = "Japanese";
            deckInfo.default_blang = "Spanish";
            deckInfo.userId = johnDoeId;
        }

        /**
        * Grabs the user ID from the JWT token
        */
        public async Task grabUserId()
        {
            // TEST 1: Get a card that belongs to John
            HttpResponseMessage response = await client.GetAsync("Cards/1");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                CardInfo card = JsonConvert.DeserializeObject<CardInfo>(contents);
                this.userId = card.ownerId;
            }
            else
            {
                throw new Exception();
            }
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

        /**
        * Response from API
        */
        public class CreatedResponse
        {
            public int id;
            public DateTime dateCreated;
        }

        /**
         * Response from API
         */
        public class UpdatedResponse
        {
            public int id;
            public DateTime dateUpdated;
        }
    }
}
