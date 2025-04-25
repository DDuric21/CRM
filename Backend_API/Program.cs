using Backend_API.Logging;
using Backend_API.Middleware;
using Backend_API.Startup;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .WriteTo.Map(
        keyPropertyName: "LogFilePath",
        configure: (logPath, wt) => wt.File($"Logs/{logPath}.log")
    )
    .CreateLogger();

builder.Host.UseSerilog();


DependencyInjectionSetup.RegisterServices(builder);

var app = builder.Build();

var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
DynamicLogger.Configure(httpContextAccessor);

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ApiPerformanceMiddleware>();
app.UseMiddleware<ApiLoggingMiddleware>();

ApplicationConfigurationSetup.ExecuteMigrations(app);
ApplicationConfigurationSetup.InitializeConfiguration(app);

app.Run();
