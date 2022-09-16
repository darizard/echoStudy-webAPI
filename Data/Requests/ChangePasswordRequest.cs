namespace echoStudy_webAPI.Data.Requests
{
    // Information required for changing a user's password
    public class ChangePasswordRequest
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}