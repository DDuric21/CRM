using AutoMapper;
using Backend_API.Handlers;
using Backend_API.Logging;
using Backend_API.Startup;

var builder = WebApplication.CreateBuilder(args);

ApplicationConfigurationSetup.ConfigurateLogger(builder);

DependencyInjectionSetup.RegisterServices(builder);

var app = builder.Build();

//var mapper = app.Services.GetRequiredService<IMapper>();
//mapper.ConfigurationProvider.AssertConfigurationIsValid();

var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
DynamicLogger.Configure(httpContextAccessor);

ApplicationConfigurationSetup.ExecuteMigrations(app);
ApplicationConfigurationSetup.InitializeConfiguration(app);

app.UseGlobalExceptionHandler();

app.Run();
