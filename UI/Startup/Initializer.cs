using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Models.Authentication;
using Resources.Translations;
using System.Globalization;
using UI.Authentication;
using UI.Services;

namespace UI.Startup
{
    public class Initializer
    {
        public void SetupCustomServices(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IAddressService, AddressService>();
            builder.Services.AddScoped<IAssetService, AssetService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IInteractionService, InteractionService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ICommunicationService, CrmCommunicationService>();
            builder.Services.AddScoped<IBillingProfileService, BillingProfileService>();
            builder.Services.AddScoped<INewsService, NewsService>();
            builder.Services.AddScoped<ILoggingService, LoggingService>();
            builder.Services.AddScoped<ICrmModalService, CrmModalService>();
            builder.Services.AddScoped<AuthenticationStateProvider, CrmAuthenticationStateProvider>();
            builder.Services.AddScoped<ICookie, Cookie>();
        }

        public void SetupHttpClient(WebAssemblyHostBuilder builder, AppConfig config)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(config.SecureBackendUrl),
            };

            builder.Services.AddScoped(sp => httpClient);
        }

        public async Task<AppConfig> SetupConfigurationAsync(WebAssemblyHostBuilder builder)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),

            };

            var stream = await httpClient.GetStreamAsync("appCustomSettings.json");
            builder.Configuration.AddJsonStream(stream);

            var config = builder.Configuration.GetSection("profiles:iisBackend").Get<AppConfig>();

            return config;
        }

        public void SetupAuthorization(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy(CrmPolicyNames.EditUser, policy =>
                    policy.RequireClaim(CrmClaimTypes.Permission, CrmPermissionNames.EditUser));
            });
        }

        public async Task SetupCultureAsync(WebAssemblyHost app)
        {
            CultureInfo culture = null;
            try
            {
                var localStorage = app.Services.GetRequiredService<ILocalStorageService>();
                var cultureInfo = await localStorage.GetItemAsync<string>("crmApplicationCulture");
                culture = !string.IsNullOrWhiteSpace(cultureInfo)
                    ? new CultureInfo(cultureInfo)
                    : new CultureInfo("en-US");
            }
            catch (Exception ex)
            {
                culture = new CultureInfo("en-US");
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Translation.Culture = culture;
        }
    }
}
