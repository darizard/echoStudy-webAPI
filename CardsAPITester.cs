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
    public class CardsAPITester
    {
        public HttpClient client;
        public string johnDoeId;

        // Determines whether localhost or api.echostudy.com is tested
        public bool testDevelopment = true;

        public CardsAPITester()
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
        * Gets all cards, should not be empty
        */
        [TestMethod]
        public async Task GetCards()
        {
            HttpResponseMessage response = await client.GetAsync("Cards");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<CardInfo> cards = JsonConvert.DeserializeObject<List<CardInfo>>(contents);
                Assert.AreNotEqual(0, cards.Count);
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
         * Gets all cards related to deck id 1, should not be empty
         */
        [TestMethod]
        public async Task GetCardsByValidDeckId()
        {
            HttpResponseMessage response = await client.GetAsync("Cards?deckId=1");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<CardInfo> cards = JsonConvert.DeserializeObject<List<CardInfo>>(contents);
                Assert.AreNotEqual(0, cards.Count);
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Gets all cards related to an invalid deck id, should be empty
        */
        [TestMethod]
        public async Task GetCardsByInvalidDeckId()
        {
            HttpResponseMessage response = await client.GetAsync("Cards?deckId=-1");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<CardInfo> cards = JsonConvert.DeserializeObject<List<CardInfo>>(contents);
                Assert.AreEqual(0, cards.Count);
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Gets all cards related to the email johndoe@gmail.com, should not be empty
        */
        [TestMethod]
        public async Task GetCardsByValidUserEmail()
        {
            HttpResponseMessage response = await client.GetAsync("Cards?userEmail=JohnDoe@gmail.com");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<CardInfo> cards = JsonConvert.DeserializeObject<List<CardInfo>>(contents);
                Assert.AreNotEqual(0, cards.Count);
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Gets all cards related to a nonexistent user, should be empty
        */
        [TestMethod]
        public async Task GetCardsByInvalidUserEmail()
        {
            HttpResponseMessage response = await client.GetAsync("Cards?userEmail=notreal@gmail.com");

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Gets all cards related to the id related to johndoe@gmail.com, should not be empty
        */
        [TestMethod]
        public async Task GetCardsByValidUserId()
        {
            HttpResponseMessage response = await client.GetAsync("Cards?userId=" + johnDoeId);

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<CardInfo> cards = JsonConvert.DeserializeObject<List<CardInfo>>(contents);
                Assert.AreNotEqual(0, cards.Count);
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Gets all cards related to a bogus id, should be empty
        */
        [TestMethod]
        public async Task GetCardsByInvalidUserId()
        {
            HttpResponseMessage response = await client.GetAsync("Cards?userId=1234567890");

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Gets a card with the id of 1, should succeed and parse
        */
        [TestMethod]
        public async Task GetCardByValidId()
        {
            HttpResponseMessage response = await client.GetAsync("Cards/1");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                CardInfo card = JsonConvert.DeserializeObject<CardInfo>(contents);
            }
            else
            {
                Assert.Fail();
            }
        }

        /**
        * Gets a card with a bogus ID, should not succeed
        */
        [TestMethod]
        public async Task GetCardByInvalidId()
        {
            HttpResponseMessage response = await client.GetAsync("Cards/-1");

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Attempts to create invalid cards through missing fields and bogus IDs
        */
        [TestMethod]
        public async Task CreateInvalidCards()
        {
            PostCardInfo cardInfo = new PostCardInfo();

            // no backlang
            populateCard(cardInfo);
            cardInfo.backLang = null;
            HttpResponseMessage response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }

            // no frontlang
            populateCard(cardInfo);
            cardInfo.frontLang = null;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }

            // no fronttext
            populateCard(cardInfo);
            cardInfo.frontText = null;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }

            // no backtext
            populateCard(cardInfo);
            cardInfo.backText = null;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }

            // no deckid
            populateCard(cardInfo);
            cardInfo.deckId = null;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }

            // no userid
            populateCard(cardInfo);
            cardInfo.userId = null;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }

            // bogus deckId
            populateCard(cardInfo);
            cardInfo.deckId = -1;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }

            // bogus userId
            populateCard(cardInfo);
            cardInfo.userId = "123456789";
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public async Task CreateValidCard()
        {
            PostCardInfo cardInfo = new PostCardInfo();
            populateCard(cardInfo);
            HttpResponseMessage response = await client.PostAsync("Cards", createContent(cardInfo));

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                CreatedResponse createdResponse = JsonConvert.DeserializeObject<CreatedResponse>(contents);
                response = await client.GetAsync("Cards/" + createdResponse.id);

                if (response.IsSuccessStatusCode)
                {
                    contents = await response.Content.ReadAsStringAsync();
                    CardInfo card = JsonConvert.DeserializeObject<CardInfo>(contents);

                    // Ensure everything is right
                    Assert.AreEqual(johnDoeId, card.ownerId);
                    Assert.AreEqual(cardInfo.deckId, card.deckId);
                    Assert.AreEqual(cardInfo.frontText, card.ftext);
                    Assert.AreEqual(cardInfo.backText, card.btext);
                    Assert.AreEqual(cardInfo.frontLang, card.flang);
                    Assert.AreEqual(cardInfo.backLang, card.blang);

                    // Delete the card
                    response = await client.PostAsync("Cards/Delete/" + createdResponse.id, null);
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
        * Edits a card with a bogus ID, should not succeed
        */
        [TestMethod]
        public async Task EditCardWithInvalidId()
        {
            // Card
            PostCardInfo cardInfo = new PostCardInfo();
            HttpResponseMessage response = await client.PostAsync("Cards/-1", createContent(cardInfo));

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Edits a card with a bogus deck ID, should not succeed
        */
        [TestMethod]
        public async Task EditCardWithInvalidDeckId()
        {
            // Card
            PostCardInfo cardInfo = new PostCardInfo();
            populateCard(cardInfo);
            cardInfo.deckId = -1;
            HttpResponseMessage response = await client.PostAsync("Cards/1", createContent(cardInfo));

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Edits a card with a bogus owner ID, should not succeed
        */
        [TestMethod]
        public async Task EditCardWithInvalidOwnerId()
        {
            // Card
            PostCardInfo cardInfo = new PostCardInfo();
            populateCard(cardInfo);
            cardInfo.userId = "1234567890";
            HttpResponseMessage response = await client.PostAsync("Cards/1", createContent(cardInfo));

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Edits a card with a valid info, should succeed
        */
        [TestMethod]
        public async Task EditCardValid()
        {
            // First, store the old info
            HttpResponseMessage response = await client.GetAsync("Cards/1");
            CardInfo oldCard = new CardInfo();
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                oldCard = JsonConvert.DeserializeObject<CardInfo>(contents);
            }
            else
            {
                Assert.Fail();
            }

            // Card
            PostCardInfo cardInfo = new PostCardInfo();
            populateCard(cardInfo);
            response = await client.PostAsync("Cards/1", createContent(cardInfo));

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                UpdatedResponse updatedResponse = JsonConvert.DeserializeObject<UpdatedResponse>(contents);
                response = await client.GetAsync("Cards/" + updatedResponse.id);

                if (response.IsSuccessStatusCode)
                {
                    contents = await response.Content.ReadAsStringAsync();
                    CardInfo card = JsonConvert.DeserializeObject<CardInfo>(contents);

                    // Ensure everything is right
                    Assert.AreEqual(johnDoeId, card.ownerId);
                    Assert.AreEqual(cardInfo.deckId, card.deckId);
                    Assert.AreEqual(cardInfo.frontText, card.ftext);
                    Assert.AreEqual(cardInfo.backText, card.btext);
                    Assert.AreEqual(cardInfo.frontLang, card.flang);
                    Assert.AreEqual(cardInfo.backLang, card.blang);

                    // Change it all back
                    cardInfo.userId = oldCard.ownerId;
                    cardInfo.deckId = oldCard.deckId;
                    cardInfo.frontText = oldCard.ftext;
                    cardInfo.backText = oldCard.btext;
                    cardInfo.frontLang = oldCard.flang;
                    cardInfo.backLang = oldCard.blang;
                    response = await client.PostAsync("Cards/1", createContent(cardInfo));
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
        * Deletes a card with a bogus ID, should not succeed
        */
        [TestMethod]
        public async Task DeleteCardByInvalidId()
        {
            HttpResponseMessage response = await client.PostAsync("Cards/Delete/-1", null);

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail();
            }
        }

        /**
        * Creates and deletes a card
        */
        [TestMethod]
        public async Task DeleteCardByValidId()
        {
            PostCardInfo cardInfo = new PostCardInfo();
            populateCard(cardInfo);
            HttpResponseMessage response = await client.PostAsync("Cards", createContent(cardInfo));

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                CreatedResponse createdResponse = JsonConvert.DeserializeObject<CreatedResponse>(contents);
                response = await client.PostAsync("Cards/Delete/" + createdResponse.id, null);
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
         * Populates given card info
         */
        private void populateCard(PostCardInfo cardInfo)
        {
            cardInfo.frontText = "To dance";
            cardInfo.backText = "tanzen";
            cardInfo.frontLang = "English";
            cardInfo.backLang = "German";
            cardInfo.userId = johnDoeId;
            cardInfo.deckId = 1;
        }

        /**
         * Creates an HttpContent object with the given card
         */
        private HttpContent createContent(PostCardInfo card)
        {
            // Serialize our concrete class into a JSON String
            var stringPayload = JsonConvert.SerializeObject(card);

            // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
            return new StringContent(stringPayload, Encoding.UTF8, "application/json");
        }

        /**
         * Returns user id associasted with JohnDoe@gmail.com
         */
        public async Task<string> getJohnDoeUserId()
        {
            // First seeded card belongs to the user JohnDoe@gmail.com
            HttpResponseMessage response = await client.GetAsync("Cards/1");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                CardInfo card = JsonConvert.DeserializeObject<CardInfo>(contents);
                return card.ownerId;
            }
            else
            {
                throw new AssertFailedException("Query for John Doe ID failed");
            }
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
            public string userId { get; set; }
            public int? deckId { get; set; }
        }
    }
}

