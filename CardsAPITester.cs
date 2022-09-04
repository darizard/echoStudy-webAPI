using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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

        // Determines whether localhost or api.echostudy.com is tested
        public bool testDevelopment = true;

        // Token for the user to be tested
        public bool userTokenSet;

        // The user's id
        public string userId;


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

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /**
        * Tests the GET Cards endpoint with all possible types of requests and validates the responses
        */
        [TestMethod]
        public async Task GetCardsTest()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
                await grabUserId();
            }

            // TEST 1: Get all of John's cards
            int total = 0;
            HttpResponseMessage response = await client.GetAsync("Cards");
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<CardInfo> allCards = JsonConvert.DeserializeObject<List<CardInfo>>(contents);
                Assert.AreNotEqual(0, allCards.Count);
                total = allCards.Count;
            }
            else
            {
                Assert.Fail("TEST 1: Request failed when trying to get all of John's cards");
            }

            // TEST 2: Get one of John's decks by ID
            response = await client.GetAsync("Cards?deckId=1");
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                List<CardInfo> deckCards = JsonConvert.DeserializeObject<List<CardInfo>>(contents);
                Assert.AreNotEqual(0, deckCards.Count);
                if(deckCards.Count >= total)
                {
                    Assert.Fail("TEST 2: Grabbing deck by ID produces same result as no parameter");
                }
            }
            else
            {
                Assert.Fail("TEST 2: Request failed when trying to get one of John's deck's cards ");
            }

            // TEST 3: Get a deck that can't be found
            response = await client.GetAsync("Cards?deckId=-1");

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 3: Success for a nonexistent deck ID");
            }
            else
            {
                if(response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("TEST 3: Status code for non existent deck wasn't NotFound");
                }
            }

            // TEST 4: Get a deck that isn't John's
            response = await client.GetAsync("Cards?deckId=4");

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 3: Success for deck John doesn't own");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.Forbidden)
                {
                    Assert.Fail("TEST 3: Status code for a deck John doesn't own wasn't Forbidden");
                }
            }
        }

        /**
        * Tests the GET Cards/{id} endpoint with all possible types of requests and validates the responses
        */
        [TestMethod]
        public async Task GetCardsIdTest()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
                await grabUserId();
            }

            // TEST 1: Get a card that belongs to John
            HttpResponseMessage response = await client.GetAsync("Cards/1");

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                CardInfo card = JsonConvert.DeserializeObject<CardInfo>(contents);
            }
            else
            {
                Assert.Fail("TEST 1: Request failed for a valid card");
            }

            // TEST 2: Get a card that doesn't exist
            response = await client.GetAsync("Cards/-1");

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 2: Request succeeded for a nonexistent card");
            }
            else
            {
                if(response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("TEST 2: Status code for a card that doesn't exist wasn't NotFound");
                }
            }

            // TEST 3: Get a card that doesn't belong to John
            response = await client.GetAsync("Cards/600");

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 2: Request succeeded for a card John doesn't own");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.Forbidden)
                {
                    Assert.Fail("TEST 2: Status code for a card that doesn't belong to John wasn't Forbidden");
                }
            }
        }


        /**
         * Tests the POST Cards/Delete/{id} endpoint with all possible types of requests and validates the responses
         */
        [TestMethod]
        public async Task PostCardsDeleteTest()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
                await grabUserId();
            }

            // TEST 1: Attempts to delete a card that doesn't exist
            HttpResponseMessage response = await client.PostAsync("Cards/Delete/600", null);

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded for a card that doesn't exist");
            }
            /*
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("TEST 2: Status code for a card that doesn't exist wasn't NotFound");
                }
            }
            */

            // TEST 2: Attempts to delete a card that John doesn't own
            response = await client.GetAsync("Cards/Delete/600");

            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 2: Request succeeded for deleting a card John doesn't own");
            }
            /*
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.Forbidden)
                {
                    Assert.Fail("TEST 2: Status code for a card that doesn't exist wasn't Forbidden");
                }
            }
            */

            // TEST 3: Create a card and then delete it and then ensure it's actually deleted
            PostCardInfo cardInfo = new PostCardInfo();
            await populateCard(cardInfo);
            response = await client.PostAsync("Cards", createContent(cardInfo));

            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                CreatedResponse createdResponse = JsonConvert.DeserializeObject<CreatedResponse>(contents);
                response = await client.PostAsync("Cards/Delete/" + createdResponse.id, null);
                if (!response.IsSuccessStatusCode)
                {
                    Assert.Fail("TEST 3: Request failed to delete a valid card");
                }
            }
            else
            {
                Assert.Fail("TEST 3: Request failed to create a card");
            }
        }

        /**
        * Tests the POST Cards endpoint with every type of request possible and validates the responses
        */
        [TestMethod]
        public async Task PostCardsCreateTest()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
                await grabUserId();
            }

            // TEST 1: Attempt to create cards with various bogus values
            PostCardInfo cardInfo = new PostCardInfo();

            // no backlang
            await populateCard(cardInfo);
            cardInfo.backLang = null;
            HttpResponseMessage response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded with empty backLang");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("TEST 1: Status code was not BadRequest for missing parameters");
                }
            }

            // no frontlang
            await populateCard(cardInfo);
            cardInfo.frontLang = null;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded with empty frontLang");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("TEST 1: Status code was not BadRequest for missing parameters");
                }
            }

            // no fronttext
            await populateCard(cardInfo);
            cardInfo.frontText = null;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded with empty frontText");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("TEST 1: Status code was not BadRequest for missing parameters");
                }
            }

            // no backtext
            await populateCard(cardInfo);
            cardInfo.backText = null;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded with empty backText");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("TEST 1: Status code was not BadRequest for missing parameters");
                }
            }

            // no deckid
            await populateCard(cardInfo);
            cardInfo.deckId = null;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded with empty deckId");
            }

            // no userid
            await populateCard(cardInfo);
            cardInfo.userId = null;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded with empty userId");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("TEST 1: Status code was not BadRequest for missing parameters");
                }
            }

            // bogus deckId
            await populateCard(cardInfo);
            cardInfo.deckId = -1;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded with invalid deckId");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("TEST 1: Status code was not NotFound for a bad deckId");
                }
            }

            // bogus userId
            /*
            await populateCard(cardInfo);
            cardInfo.userId = "123456789";
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded with invalid userId");
            }
            */

            // Access a deck John doesn't own
            await populateCard(cardInfo);
            cardInfo.deckId = 5;
            response = await client.PostAsync("Cards", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded with another user's deckId");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.Forbidden)
                {
                    Assert.Fail("TEST 1: Status code was not Forbidden for creating a card in someone else's deck");
                }
            }

            // TEST 2: Create valid cards
            await populateCard(cardInfo);
            response = await client.PostAsync("Cards", createContent(cardInfo));

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
                    Assert.AreEqual(this.userId, card.ownerId);
                    Assert.AreEqual(cardInfo.deckId, card.deckId);
                    Assert.AreEqual(cardInfo.frontText, card.ftext);
                    Assert.AreEqual(cardInfo.backText, card.btext);
                    Assert.AreEqual(cardInfo.frontLang, card.flang);
                    Assert.AreEqual(cardInfo.backLang, card.blang);

                    // Delete the card
                    response = await client.PostAsync("Cards/Delete/" + createdResponse.id, null);
                    if (!response.IsSuccessStatusCode)
                    {
                        Assert.Fail("TEST 2: Request failed to delete card that was created");
                    }
                }
                else
                {
                    Assert.Fail("TEST 2: Request failed to get created card");
                }
            }
            else
            {
                Assert.Fail("TEST 2: Request failed to create card");
            }
        }

        /**
        * Tests POST Cards/{id} endpoint with every possible request and validates the responses
        */
        [TestMethod]
        public async Task PostEditCardsTest()
        {
            // Ensure John's token is set for the client first
            if (!userTokenSet)
            {
                await GrabJohnToken();
                await grabUserId();
            }

            // TEST 1: Edit a card with an invalid id
            PostCardInfo cardInfo = new PostCardInfo();
            HttpResponseMessage response = await client.PostAsync("Cards/-1", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded for a nonexistent card");
            }

            // TEST 2: Edit a card with an invalid deckId
            // Card
            cardInfo = new PostCardInfo();
            await populateCard(cardInfo);
            cardInfo.deckId = -1;
            response = await client.PostAsync("Cards/1", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded for a nonexistent deckId");
            }

            // TEST 3: Edit a card with an invalid userId
            // Card
            cardInfo = new PostCardInfo();
            await populateCard(cardInfo);
            cardInfo.userId = "1234567890";
            response = await client.PostAsync("Cards/1", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded for a bogus userId");
            }

            // TEST 4: Edit a card that someone else owns
            // Card
            cardInfo = new PostCardInfo();
            await populateCard(cardInfo);
            response = await client.PostAsync("Cards/600", createContent(cardInfo));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("TEST 1: Request succeeded for a bogus userId");
            }

            // TEST 5: Edit a valid card
            // First, store the old info
            response = await client.GetAsync("Cards/1");
            CardInfo oldCard = new CardInfo();
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                oldCard = JsonConvert.DeserializeObject<CardInfo>(contents);
            }
            else
            {
                Assert.Fail("TEST 5: Failed to grab a card that should exist");
            }

            // Card
            cardInfo = new PostCardInfo();
            await populateCard(cardInfo);
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
                    Assert.AreEqual(this.userId, card.ownerId);
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
                        Assert.Fail("TEST 5: Request failed to restore the old card after editing");
                    }
                }
                else
                {
                    Assert.Fail("TEST 5: Request failed to get the edited card");
                }
            }
            else
            {
                Assert.Fail("TEST 5: Request failed to edit a card");
            }
        }

        /**
         * Populates given card info
         */
        private async Task populateCard(PostCardInfo cardInfo)
        {
            cardInfo.frontText = "To dance";
            cardInfo.backText = "tanzen";
            cardInfo.frontLang = "English";
            cardInfo.backLang = "German";
            cardInfo.userId = this.userId;
            cardInfo.deckId = 1;
        }

        /**
         * Grabs the user ID from the JWT token
         */
        private async Task grabUserId()
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
        * Gets John's token, should not be empty
         */
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
         * User Info
         */
        public class UserInfo
        {
            public string username;
            public string password;
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

