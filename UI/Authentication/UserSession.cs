namespace UI.Authentication
{
    public class UserSession
    {
        public string UserName { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public IEnumerable<string> Permissions { get; set; }
    }
}
