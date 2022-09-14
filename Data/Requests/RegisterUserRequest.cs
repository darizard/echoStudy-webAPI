namespace echoStudy_webAPI.Data.Requests
{
    // information used in creating a new user
    public class RegisterUserRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}
