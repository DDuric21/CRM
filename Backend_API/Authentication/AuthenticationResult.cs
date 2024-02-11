namespace Backend_API.Authentication
{
    public class AuthenticationResult
    {
        public string Token { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<string> ErrorMessages { get; set; }
        public string RefreshToken { get; set; }

        public AuthenticationResult()
        {
            ErrorMessages = new List<string>();
        }

    }
}
