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
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Newtonsoft.Json.Linq;

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
        * Tests the GET users endpoint with all possible types of requests
        */
        [TestMethod]
        public async Task PostGetUserTest()
        {
            // Try to get a user with no token
            HttpResponseMessage response = await client.GetAsync("users");
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("No token request succeeded");
            }

            // Get a user with the token set
            await HelperFunctions.GrabJohnToken(client);
            response = await client.GetAsync("users");
            if (response.IsSuccessStatusCode)
            {
                var contents = await response.Content.ReadAsStringAsync();
                UserInfoResponse userInfo = JsonConvert.DeserializeObject<UserInfoResponse>(contents);
                Assert.IsNotNull(userInfo.Username);
                Assert.IsNotNull(userInfo.Email);
                Assert.IsNotNull(userInfo.PhoneNumber);
            }
            else
            {
                Assert.Fail("Could not obtain John's user information");
            }
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

            // Register the user, send edit requests, deregister user
            if (response.IsSuccessStatusCode)
            {
                // Try to edit with invalid password
                registerUserRequest.Password = "fakepassword";
                response = await client.PostAsync("users", new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    Assert.Fail("Invalid password request succeeded");
                }
                else
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                    {
                        Assert.Fail("Invalid password did not yield a unauthorized response");
                    }
                }
                registerUserRequest.Password = "123ABC!@#def";

                // Legit edit request
                registerUserRequest.PhoneNumber = "0987-654-321";
                userContent = new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json");
                response = await client.PostAsync("users", userContent);
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

            // empty password
            registerUserRequest.Password = null;
            response = await client.PostAsync("users", new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Null password request succeeded");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("Null password did not yield a bad request response");
                }
            }
            registerUserRequest.Password = "123ABC!@#def";

            // empty email and username
            registerUserRequest.Email = null;
            registerUserRequest.UserName = null;
            response = await client.PostAsync("users", new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Null email/username request succeeded");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("Null email/username did not yield a bad request response");
                }
            }
            registerUserRequest.Email = "echotestuser12345@gmail.com";
            registerUserRequest.UserName = "echotestuser12345";

            // invalid email
            registerUserRequest.Email = "notreal@notreal.moc";
            response = await client.PostAsync("users", new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Invalid email request succeeded");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("Invalid email did not yield a bad request response");
                }
            }
            registerUserRequest.Email = "echotestuser12345@gmail.com";

            // invalid username
            registerUserRequest.UserName = "notreal@notreal.moc";
            response = await client.PostAsync("users", new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Invalid username request succeeded");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("Invalid username did not yield a bad request response");
                }
            }
            registerUserRequest.UserName = "echotestuser12345";
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

            ChangePasswordRequest changePasswordRequest = new ChangePasswordRequest();
            changePasswordRequest.Email = "echotestuser12345@gmail.com";
            changePasswordRequest.OldPassword = "123ABC!@#def";
            changePasswordRequest.NewPassword = "NewPassword123!@#";
            changePasswordRequest.PhoneNumber = "123-456-7890";

            // Register the user, make requests to change password, delete the user
            HttpContent userContent = new StringContent(JsonConvert.SerializeObject(registerUserRequest), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("register", userContent);
            if (response.IsSuccessStatusCode)
            {
                // Trying to change to same passwords
                changePasswordRequest.OldPassword = changePasswordRequest.NewPassword;
                response = await client.PostAsync("changepassword", new StringContent(JsonConvert.SerializeObject(changePasswordRequest), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    Assert.Fail("Identical passwords request succeeded");
                }
                else
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                    {
                        Assert.Fail("Identical passwords request didn't return BadRequest");
                    }
                }
                changePasswordRequest.OldPassword = changePasswordRequest.OldPassword = "123ABC!@#def";

                // Trying to change password with incorrect phonenumber
                changePasswordRequest.PhoneNumber = "321-456-7890";
                response = await client.PostAsync("changepassword", new StringContent(JsonConvert.SerializeObject(changePasswordRequest), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    Assert.Fail("Non-matching phone number succeeded");
                }
                else
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                    {
                        Assert.Fail("Non-matching phone number did not return bad request");
                    }
                }
                changePasswordRequest.PhoneNumber = "123-456-7890";

                // Trying to change password with an incorrect old password
                changePasswordRequest.OldPassword = "incorrectpassword";
                response = await client.PostAsync("changepassword", new StringContent(JsonConvert.SerializeObject(changePasswordRequest), Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    Assert.Fail("Incorrect old password request succeeded");
                }
                else
                {
                    if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                    {
                        Assert.Fail("Incorrect old password did not yield unauthorized result");
                    }
                }
                changePasswordRequest.OldPassword = "123ABC!@#def";

                // Changing password normally
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

            // null email
            changePasswordRequest.Email = null;
            response = await client.PostAsync("changepassword", new StringContent(JsonConvert.SerializeObject(changePasswordRequest), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Null email request succeeded");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("Null email did not yield a bad request response");
                }
            }
            changePasswordRequest.Email = "echotestuser12345@gmail.com";

            // null phone number
            changePasswordRequest.PhoneNumber = null;
            response = await client.PostAsync("changepassword", new StringContent(JsonConvert.SerializeObject(changePasswordRequest), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Null phone number request succeeded");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("Null phone number did not yield a bad request response");
                }
            }
            changePasswordRequest.PhoneNumber = "123-456-7890";

            // null old password
            changePasswordRequest.OldPassword = null;
            response = await client.PostAsync("changepassword", new StringContent(JsonConvert.SerializeObject(changePasswordRequest), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Null old password request succeeded");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("Null old password did not yield a bad request response");
                }
            }
            changePasswordRequest.OldPassword = "123ABC!@#def";

            // null new password
            changePasswordRequest.NewPassword = null;
            response = await client.PostAsync("changepassword", new StringContent(JsonConvert.SerializeObject(changePasswordRequest), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Null new password request succeeded");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.BadRequest)
                {
                    Assert.Fail("Null new password did not yield a bad request response");
                }
            }
            changePasswordRequest.NewPassword = "NewPassword123!@#";

            // non-existent user
            response = await client.PostAsync("changepassword", new StringContent(JsonConvert.SerializeObject(changePasswordRequest), Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Assert.Fail("Non-existent user request succeeded");
            }
            else
            {
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    Assert.Fail("Non-existent user request did not return NotFound");
                }
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
