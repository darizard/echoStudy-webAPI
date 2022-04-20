using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Tests
{
    [TestClass]
    public class DeckAPITester
    {
        public HttpClient client;
        public string johnDoeId;

        // Determines whether localhost or api.echostudy.com is tested
        public bool testDevelopment = true;

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
            johnDoeId = getJohnDoeUserId().Result;
        }

        /**
        * Gets all decks, should not be empty
        */
        [TestMethod]
        public async Task GetDecks()
        {
            HttpResponseMessage response = await client.GetAsync("Decks");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<DeckInfo> decks = JsonConvert.DeserializeObject<List<DeckInfo>>(contents);
                Assert.AreNotEqual(0, decks.Count);
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Gets all public decks, should not be empty
        */
        [TestMethod]
        public async Task GetPublicDecks()
        {
            HttpResponseMessage response = await client.GetAsync("Decks/Public");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<DeckInfo> decks = JsonConvert.DeserializeObject<List<DeckInfo>>(contents);
                Assert.AreNotEqual(0, decks.Count);
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Gets all decks related to the email johndoe@gmail.com, should not be empty
        */
        [TestMethod]
        public async Task GetDecksByValidUserEmail()
        {
            HttpResponseMessage response = await client.GetAsync("Decks?userEmail=JohnDoe@gmail.com");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<DeckInfo> decks = JsonConvert.DeserializeObject<List<DeckInfo>>(contents);
                Assert.AreNotEqual(0, decks.Count);
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Gets all decks related to a nonexistent user, should be empty
        */
        [TestMethod]
        public async Task GetDecksByInvalidUserEmail()
        {
            HttpResponseMessage response = await client.GetAsync("Decks?userEmail=notreal@gmail.com");

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Gets all decks related to the id related to johndoe@gmail.com, should not be empty
        */
        [TestMethod]
        public async Task GetDecksByValidUserId()
        {
            HttpResponseMessage response = await client.GetAsync("Decks?userId=" + johnDoeId);

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<DeckInfo> decks = JsonConvert.DeserializeObject<List<DeckInfo>>(contents);
                Assert.AreNotEqual(0, decks.Count);
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Gets all decks related to a bogus id, should be empty
        */
        [TestMethod]
        public async Task GetDecksByInvalidUserId()
        {
            HttpResponseMessage response = await client.GetAsync("Decks?userId=1234567890");

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Returns user id associasted with JohnDoe@gmail.com
        */
        public async Task<string> getJohnDoeUserId()
        {
            // First seeded deck belongs to the user JohnDoe@gmail.com
            HttpResponseMessage response = await client.GetAsync("Decks/1");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                DeckInfo deck = JsonConvert.DeserializeObject<DeckInfo>(contents);
                return deck.ownerId;
            }
            else
            {
                throw new AssertFailedException("Query for John Doe ID failed");
            }
        }

        /**
        * Gets a deck with the id of 1, should succeed and parse
        */
        [TestMethod]
        public async Task GetDeckByValidId()
        {
            HttpResponseMessage response = await client.GetAsync("Decks/1");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                DeckInfo deck = JsonConvert.DeserializeObject<DeckInfo>(contents);
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Gets a deck with a bogus ID, should not succeed
        */
        [TestMethod]
        public async Task GetDeckByInvalidId()
        {
            HttpResponseMessage response = await client.GetAsync("Decks/-1");

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Attempts to create invalid decks through missing fields and bogus IDs
        */
        [TestMethod]
        public async Task CreateInvalidDecks()
        {
            PostDeckInfo deckInfo = new PostDeckInfo();

            // no backlang
            populateDeck(deckInfo);
            deckInfo.title = null;
            HttpResponseMessage response = await client.PostAsync("Decks", createContent(deckInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }

            // no frontlang
            populateDeck(deckInfo);
            deckInfo.description = null;
            response = await client.PostAsync("Decks", createContent(deckInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }

            // no fronttext
            populateDeck(deckInfo);
            deckInfo.default_blang = null;
            response = await client.PostAsync("Decks", createContent(deckInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }

            // no backtext
            populateDeck(deckInfo);
            deckInfo.default_flang = null;
            response = await client.PostAsync("Decks", createContent(deckInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }

            // no deckid
            populateDeck(deckInfo);
            deckInfo.userId = null;
            response = await client.PostAsync("Decks", createContent(deckInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
         * Creates a new deck, should succeed
         */
        [TestMethod]
        public async Task CreateValidDeck()
        {
            PostDeckInfo deckInfo = new PostDeckInfo();
            populateDeck(deckInfo);
            HttpResponseMessage response = await client.PostAsync("Decks", createContent(deckInfo));

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
                    Assert.AreEqual(johnDoeId, deck.ownerId);
                    Assert.AreEqual(deckInfo.title, deck.title);
                    Assert.AreEqual(deckInfo.description, deck.description);
                    Assert.AreEqual(deckInfo.default_flang, deck.default_flang);
                    Assert.AreEqual(deckInfo.default_blang, deck.default_blang);

                    // Delete the deck
                    response = await client.PostAsync("Decks/Delete/" + createdResponse.id, null);
                    if (!response.IsSuccessStatusCode)
                    {
                        Assert.Fail();
                    }
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Edits a deck with a bogus ID, should not succeed
        */
        [TestMethod]
        public async Task EditDeckWithInvalidId()
        {
            // Deck
            PostDeckInfo deckInfo = new PostDeckInfo();
            HttpResponseMessage response = await client.PostAsync("Decks/-1", createContent(deckInfo));

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Edits a deck with a bogus owner ID, should not succeed
        */
        [TestMethod]
        public async Task EditDeckWithInvalidOwnerId()
        {
            // Deck
            PostDeckInfo deckInfo = new PostDeckInfo();
            populateDeck(deckInfo);
            deckInfo.userId = "1234567890";
            HttpResponseMessage response = await client.PostAsync("Decks/1", createContent(deckInfo));

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Edits a deck with a valid info, should succeed
        */
        [TestMethod]
        public async Task EditDeckValid()
        {
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
                Assert.Fail();
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
                    Assert.AreEqual(johnDoeId, deck.ownerId);
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
                        Assert.Fail();
                    }
                }
                else
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Deletes a deck with a bogus ID, should not succeed
        */
        [TestMethod]
        public async Task DeleteDeckByInvalidId()
        {
            HttpResponseMessage response = await client.PostAsync("Decks/Delete/-1", null);

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Creates and deletes a deck
        */
        [TestMethod]
        public async Task DeleteDeckByValidId()
        {
            PostDeckInfo deckInfo = new PostDeckInfo();
            populateDeck(deckInfo);
            HttpResponseMessage response = await client.PostAsync("Decks", createContent(deckInfo));

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                CreatedResponse createdResponse = JsonConvert.DeserializeObject<CreatedResponse>(contents);
                response = await client.PostAsync("Decks/Delete/" + createdResponse.id, null);
                if (!response.IsSuccessStatusCode)
                {
                    Assert.Fail();
                }
            }
            else
            {
                Assert.Fail();
            }
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
