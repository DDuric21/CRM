using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using UI;
using UI.Startup;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var initializer = new Initializer();

initializer.SetupCustomServices(builder);

var configuration = await initializer.SetupConfigurationAsync(builder);
builder.Services.AddSingleton(configuration);

initializer.SetupHttpClient(builder, configuration);

initializer.SetupAuthorization(builder);

builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();

await initializer.SetupCultureAsync(app);

await app.RunAsync();
