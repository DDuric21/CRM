namespace Backend_API.Services.Implementations
{
    public static class DiContainer
    {
        public static IServiceProvider Provider { get; private set; }

        public static void Initialize(IServiceProvider provider)
        {
            Provider = provider;
        }
    }
}
