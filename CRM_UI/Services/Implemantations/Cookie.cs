using Microsoft.JSInterop;

namespace CRM_UI.Services
{
    public class Cookie : ICookie
    {
        readonly IJSRuntime JSRuntime;

        public Cookie(IJSRuntime jsRuntime)
        {
            JSRuntime = jsRuntime;
        }

        public async Task SetValue(string key, string value, TimeSpan? time = null)
        {
            var curExp = (time != null) 
                ? (ExpirationToUTC(time.Value)) 
                : TimeSpan.FromMinutes(15).ToString();
            await SetCookie($"{key}={value}; expires={curExp}; path=/");
        }

        public async Task<string> GetValue(string key, string def = "")
        {
            var cValue = await GetCookie();
            if (string.IsNullOrEmpty(cValue))
            {
                return def;
            }

            var vals = cValue.Split(';');
            foreach (var val in vals)
            {
                if (!string.IsNullOrEmpty(val) && val.IndexOf('=') > 0)
                {
                    if (val.Substring(0, val.IndexOf('=')).Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                    {
                        return val.Substring(val.IndexOf('=') + 1);
                    }
                }
            }

            return def;
        }

        private async Task SetCookie(string value)
        {
            await JSRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{value}\"");
        }

        private async Task<string> GetCookie()
        {
            return await JSRuntime.InvokeAsync<string>("eval", $"document.cookie");
        }

        private string ExpirationToUTC(TimeSpan time) => DateTime.Now.Add(time).ToUniversalTime().ToString("R");
    }
}
