using Backend_API.Authentication;
using Backend_API.Data.DbContext;
using Backend_API.Data.Repositories;
using Backend_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Backend_API.Startup
{
    public class DependencyInjectionSetup
    {
        public static void RegisterServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register CRM services
            builder.Services.AddDbContext<CrmDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddScoped<ICrmRepository, CrmRepository>();

            builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfig"));
            builder.Services.AddDefaultIdentity<IdentityUser>(
                options =>
                {
                    options.SignIn.RequireConfirmedEmail = false;
                })
                .AddEntityFrameworkStores<CrmDbContext>();
            builder.Services.AddAuthentication(
                options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwt =>
                {
                    var secret = builder.Configuration.GetSection("JwtConfiguration:Secret").Value;
                    var key = Encoding.ASCII.GetBytes(secret);

                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true
                    };
                });

            builder.Services.AddScoped<ILoginService, LoginService>();
        }
    }
}
