using Backend_API.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

DependencyInjectionSetup.RegisterServices(builder);

var app = builder.Build();

ApplicationConfigurationSetup.ExecuteMigrations(app);
ApplicationConfigurationSetup.InitializeConfiguration(app);

app.Run();
