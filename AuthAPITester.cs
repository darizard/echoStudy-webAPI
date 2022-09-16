using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using static echoStudy_webAPI.Tests.CardsAPITester;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static echoStudy_webAPI.Tests.Models.Models;

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
        * Tests the POST edituser endpoint with all possible types of requests
        */
        [TestMethod]
        public async Task PostEditUserTest()
        {
            RegisterUserRequest registerUserRequest = new RegisterUserRequest();
            registerUserRequest.Email = "echotestuser12345@gmail.com";
            registerUserRequest.Password = "123ABC!@#def";
            registerUserRequest.PhoneNumber = "123-456-7890";
            registerUserRequest.UserName = "echotestuser12345";

            HttpContent userContent = new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("register", userContent);

            if (response.IsSuccessStatusCode)
            {
                registerUserRequest.PhoneNumber = "0";
                userContent = new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json");
                response = await client.PostAsync("edituser", userContent);
                if (!response.IsSuccessStatusCode)
                {
                    Assert.Fail("Failed to edit user");
                }

                userContent = new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json");
                response = await client.PostAsync("deregister", userContent);
                if (!response.IsSuccessStatusCode)
                {
                    Assert.Fail("Failed to delete edited user");
                }
            }
            else
            {
                Assert.Fail("Failed to register user");
            }
        }

        /**
        * Tests the POST changepassword endpoint with all possible types of requests
        */
        [TestMethod]
        public async Task PostChangePasswordTest()
        {
            RegisterUserRequest registerUserRequest = new RegisterUserRequest();
            registerUserRequest.Email = "echotestuser12345@gmail.com";
            registerUserRequest.Password = "123ABC!@#def";
            registerUserRequest.PhoneNumber = "123-456-7890";
            registerUserRequest.UserName = "echotestuser12345";

            HttpContent userContent = new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("register", userContent);

            if (response.IsSuccessStatusCode)
            {
                ChangePasswordRequest changePasswordRequest = new ChangePasswordRequest();
                changePasswordRequest.Email = "echotestuser12345@gmail.com";
                changePasswordRequest.OldPassword = "123ABC!@#def";
                changePasswordRequest.NewPassword = "NewPassword123!@#";
                changePasswordRequest.PhoneNumber = "123-456-7890";

                userContent = new StringContent(JsonConvert.SerializeObject(changePasswordRequest), Encoding.UTF8, "application/json");
                response = await client.PostAsync("changepassword", userContent);

                if (!response.IsSuccessStatusCode)
                {
                    Assert.Fail("Request failed to change password");
                }

                registerUserRequest.Password = "NewPassword123!@#";
                userContent = new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json");
                response = await client.PostAsync("deregister", userContent);

                if (!response.IsSuccessStatusCode)
                {
                    Assert.Fail("Failed to deregister user with new password");
                }
            }
            else
            {
                Assert.Fail("Failed to register user");
            }
        }

        /**
        * Tests the POST register and POST delete endpoint with all possible types of requests
        */
        [TestMethod]
        public async Task PostRegisterDeregisterTest()
        {
            RegisterUserRequest registerUserRequest = new RegisterUserRequest();
            registerUserRequest.Email = "echotestuser12345@gmail.com";
            registerUserRequest.Password = "123ABC!@#def";
            registerUserRequest.PhoneNumber = "123-456-7890";
            registerUserRequest.UserName = "echotestuser12345";

            HttpContent userContent = new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("register", userContent);

            if (response.IsSuccessStatusCode)
            {
                // Attempt to get a token
                UserInfo userDetails = new UserInfo();
                userDetails.username = registerUserRequest.Email;
                userDetails.password = registerUserRequest.Password;

                // Get the token from the server
                HttpContent signInContent = new StringContent(JsonConvert.SerializeObject(userDetails), Encoding.UTF8, "application/json");
                response = await client.PostAsync("authenticate", signInContent);

                // Should be a successful request
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response
                    authResponse userToken = JsonConvert.DeserializeObject<authResponse>(await response.Content.ReadAsStringAsync());

                    // Make sure the token can be parsed
                    var handler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = handler.ReadJwtToken(userToken.Token);

                    // Make sure the refresh token works
                    HttpContent refreshContent = new StringContent(JsonConvert.SerializeObject(userToken), Encoding.UTF8, "application/json");
                    response = await client.PostAsync("refresh", refreshContent);
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response
                        userToken = JsonConvert.DeserializeObject<authResponse>(await response.Content.ReadAsStringAsync());

                        // Make sure the token can be parsed
                        var jwtRefreshedSecurityToken = handler.ReadJwtToken(userToken.Token);
                    }
                    else
                    {
                        Assert.Fail("Valid Post Refresh request failed to grab user token");
                    }
                }
                else
                {
                    Assert.Fail("Valid Post Authenticate request failed to grab a valid user");
                }

                userContent = new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json");
                response = await client.PostAsync("deregister", userContent);

                if (!response.IsSuccessStatusCode)
                {
                    Assert.Fail("Failed to delete registered user");
                }
            }
            else
            {
                Assert.Fail("Failed to register user");
            }
        }

        /**
        * Tests the POST authenticate and POST refresh endpoint with all possible types of requests
        */
        [TestMethod]
        public async Task PostAuthorizationRefreshTest()
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
                // Parse the response
                authResponse userToken = JsonConvert.DeserializeObject<authResponse>(await response.Content.ReadAsStringAsync());

                // Make sure the token can be parsed
                var handler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = handler.ReadJwtToken(userToken.Token);

                // Make sure the refresh token works
                HttpContent refreshContent = new StringContent(JsonConvert.SerializeObject(userToken), Encoding.UTF8, "application/json");
                response = await client.PostAsync("refresh", refreshContent);
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response
                    userToken = JsonConvert.DeserializeObject<authResponse>(await response.Content.ReadAsStringAsync());

                    // Make sure the token can be parsed
                    var jwtRefreshedSecurityToken = handler.ReadJwtToken(userToken.Token);
                }
                else
                {
                    Assert.Fail("Valid Post Refresh request failed to grab user token");
                }
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

            // Attempt to get a refresh token with bogus content
            authResponse fakeToken = new authResponse();
            fakeToken.RefreshToken = "2803cxr4xnm8940";
            fakeToken.Token = "293s0jx002m3mcx-z0wk0ak";
            signInContent = new StringContent(JsonConvert.SerializeObject(fakeToken), Encoding.UTF8, "application/json");
            response = await client.PostAsync("refresh", signInContent);

            // Should be an unsuccessful request
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Invalid Post Refresh succeeded in grabbing an invalid user");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("Post Refresh does not return unauthorized upon failure");
                }
            }
        }
    }
}
