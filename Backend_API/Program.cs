using Backend_API.Startup;

var builder = WebApplication.CreateBuilder(args);

DependencyInjectionSetup.RegisterServices(builder);

var app = builder.Build();

ApplicationConfigurationSetup.ExecuteMigrations(app);
ApplicationConfigurationSetup.InitialzeConfiguration(app);

app.Run();
