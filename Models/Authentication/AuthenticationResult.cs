namespace Models.Authentication
{
    public class AuthenticationResult
    {
        public string AccessToken { get; set; }

        public bool IsAuthenticated { get; set; }

        public List<string> ErrorMessages { get; set; }

        public string RefreshToken { get; set; }

        public AuthenticationResult()
        {
            ErrorMessages = new List<string>();
        }
    }
}
