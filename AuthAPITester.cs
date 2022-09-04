using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using static echoStudy_webAPI.Tests.CardsAPITester;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace echoStudy_webAPI.Tests
{
    [TestClass]
    public class AuthAPITester
    {
        public HttpClient client;
        public bool testDevelopment = true;

        public AuthAPITester()
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
        * Ensures authorization post succeeds and fails properly
        */
        [TestMethod]
        public async Task PostAuthenticateTest()
        {
            // The valid user
            UserInfo userDetails = new UserInfo();
            userDetails.username = "JohnDoe@gmail.com";
            userDetails.password = "123ABC!@#def";

            // Get the token from the server
            HttpContent signInContent = new StringContent(JsonConvert.SerializeObject(userDetails), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("authenticate", signInContent);

            // Should be a successful request
            if (response.IsSuccessStatusCode)
            {
                // Parse the token 
                string userToken = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(userToken);
            }
            else
            {
                Assert.Fail("Valid Post Authenticate request failed to grab a valid user");
            }

            // The invalid user
            userDetails.username = "nonsense234567@gmail.com";
            userDetails.password = "iamnotreal";

            // Attempt to get a token from the server
            signInContent = new StringContent(JsonConvert.SerializeObject(userDetails), Encoding.UTF8, "application/json");
            response = await client.PostAsync("authenticate", signInContent);

            // Should be an unsuccessful request
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Invalid Post Authenticate succeeded in grabbing an invalid user");
            }
            else
            {
                if(response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                {
                    Assert.Fail("Post Authenticate does not return unauthorized upon failure");
                }
            }
        }
    }
}
