namespace UI
{
    public sealed class AppConfig
    {
        public string Environment { get; set; }

        public string BackendUrl { get; set; }

        public string SecureBackendUrl { get; set; }

        public bool IsDevelopment()
        {
            return Environment == "Development";
        }
    }
}
