using Backend_API.Data.DbContext;
using Backend_API.Data.SeedData;
using Microsoft.EntityFrameworkCore;

namespace Backend_API.Startup
{
    public static class ApplicationConfigurationSetup
    {
        public static void InitialzeConfiguration(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                SeedData.InsertSeedData(app);
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthorization();

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
    }
}
