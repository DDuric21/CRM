using Backend_API.Handlers;
using Backend_API.Logging;
using Backend_API.Startup;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

ApplicationConfigurationSetup.ConfigurateLogger(builder);
ApplicationConfigurationSetup.ConfigureLocalization(builder);
ApplicationConfigurationSetup.ConfigureMessageBroker(builder);

DependencyInjectionSetup.RegisterServices(builder);

var app = builder.Build();

var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
DynamicLogger.Configure(httpContextAccessor);

ApplicationConfigurationSetup.ExecuteMigrations(app);
ApplicationConfigurationSetup.InitializeConfiguration(app);

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseGlobalExceptionHandler();

app.Run();
