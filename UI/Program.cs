using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Models.Authentication;
using UI;
using UI.Authentication;
using UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var httpClient = new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),

};

var stream = await httpClient.GetStreamAsync("appCustomSettings.json");
builder.Configuration.AddJsonStream(stream);
var config = builder.Configuration.GetSection("profiles:iisBackend").Get<ApiConfig>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(config.SecureBackendUrl) });

builder.Services.AddSingleton(config);

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInteractionService, InteractionService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ICommunicationService, CommunicationService>();
builder.Services.AddScoped<IBillingProfileService, BillingProfileService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<ICrmModalService, CrmModalService>();
builder.Services.AddScoped<AuthenticationStateProvider, CrmAuthenticationStateProvider>();
builder.Services.AddScoped<ICookie, Cookie>();

builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy(CrmPolicyNames.EditUser, policy =>
        policy.RequireClaim(CrmClaimTypes.Permission, CrmPermissionNames.EditUser));
});

builder.Services.AddBlazoredModal();

await builder.Build().RunAsync();
