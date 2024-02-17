namespace CRM_UI.Services
{
    public interface ICookie
    {
        Task SetValue(string key, string value, TimeSpan? time = null);
        Task<string> GetValue(string key, string def = "");
    }
}
