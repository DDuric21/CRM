using Backend_API.Data.DbContext;
using Backend_API.Data.SeedData;
using Backend_API.Middleware;
using Backend_API.Properties;
using Backend_API.Services;
using Backend_API.Services.Implementations;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;

namespace Backend_API.Startup
{
    public static class ApplicationConfigurationSetup
    {
        public static void InitializeConfiguration(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                SeedData.InsertSeedData(app);
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("LocalPolicy");

            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<ApiPerformanceMiddleware>();
            app.UseMiddleware<ApiLoggingMiddleware>();

            app.MapControllers();
        }

        public static void ExecuteMigrations(WebApplication app)
        {
            // Do migrations on startup
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
                context.Database.Migrate();
            }
        }

        internal static void ConfigurateLogger(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .WriteTo.Map(
                    keyPropertyName: "LogFilePath",
                    configure: (logPath, wt) => wt.File(
                        path: $"Logs/{logPath}.log",
                        outputTemplate: builder.Configuration["Serilog:OutputTemplate"])
                )
                .CreateLogger();

            builder.Host.UseSerilog();
        }

        internal static void ConfigureLocalization(WebApplicationBuilder builder)
        {
            builder.Services.AddLocalization(opts => opts.ResourcesPath = "Resources/Translations/API");

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("hr-HR"),
            };

            builder.Services.Configure<RequestLocalizationOptions>(opts =>
            {
                opts.DefaultRequestCulture = new RequestCulture("en-US");
                opts.SupportedCultures = supportedCultures;
                opts.SupportedUICultures = supportedCultures;
                // read from Accept-Language header:
                opts.RequestCultureProviders = new[] { new AcceptLanguageHeaderRequestCultureProvider() };
            });
        }

        internal static void ConfigureMessageBroker(WebApplicationBuilder builder)
        {
            builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));

            builder.Services.AddSingleton<RabbitMqService>();
            builder.Services.AddSingleton<IMessageBrokerService>(sp => sp.GetRequiredService<RabbitMqService>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<RabbitMqService>());
        }
    }
}
