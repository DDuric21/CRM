using Backend_API.Data.DbContext;
using Backend_API.Data.SeedData;
using Backend_API.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

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
    }
}
