using Backend_API.Data.DbContext;
using Backend_API.Data.Models;
using Backend_API.Data.Repositories;
using Backend_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models.Authentication;
using Models.Authentication.DataStructures;
using Newtonsoft.Json;
using System.Text;

namespace Backend_API.Startup
{
    public class DependencyInjectionSetup
    {
        public static void RegisterServices(WebApplicationBuilder builder)
        {
            var _config = builder.Configuration;
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer {token}'",
                    Name = HttpHeaderNames.Authorization,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement 
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            builder.Services.AddDbContext<CrmDbContext>(options => { options.UseSqlServer(_config.GetConnectionString("DefaultConnection")); });
            builder.Services.AddScoped<ICrmRepository, CrmRepository>();

            Register3AServices(builder);

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IAddressService, AddressService>();
            builder.Services.AddScoped<IAssetService, AssetService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IInteractionService, InteractionService>();
            builder.Services.AddScoped<IBillingProfileService, BillingProfileService>();
            builder.Services.AddScoped<INewsService, NewsService>();
            builder.Services.AddScoped<IOptionService, OptionService>();
            builder.Services.AddSingleton<IDataValidationService, DataValidationService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("LocalPolicy",
                    policy =>
                    {
                        var allowedOrigins = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string[]>();
                        policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();   
                    });
            });

            builder.Services.AddAutoMapper(typeof(Program).Assembly);
        }

        private static void Register3AServices(WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<User, IdentityRole>(
                options =>
                {
                    options.User.AllowedUserNameCharacters = null;
                    options.SignIn.RequireConfirmedEmail = false;
                })
                .AddUserManager<CrmUserManager>()
                .AddRoleManager<CrmRoleManager>()
                .AddEntityFrameworkStores<CrmDbContext>();

            builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection("JwtConfiguration"));

            var tokenValidationParameters = DefineTokenValidationParameters(builder);

            builder.Services.AddAuthentication(
                options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwt =>
                {
                    jwt.SaveToken = true;
                    jwt.TokenValidationParameters = tokenValidationParameters;
                    jwt.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Authentication failed: " +
                                $"\nMessage: {context.Exception.Message}" +
                                $"\nInnerException: {context.Exception.InnerException}" +
                                $"\nData: {string.Join(',', context.Exception.Data)}");
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            Console.WriteLine($"JWT Authentication Challenge triggered.");
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddSingleton(tokenValidationParameters);
        }

        private static TokenValidationParameters DefineTokenValidationParameters(WebApplicationBuilder builder)
        {
            var jwtConfig = builder.Configuration.GetSection("JwtConfiguration").Get<JwtConfiguration>();
            var key = Encoding.UTF8.GetBytes(jwtConfig.Secret);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudiences = new[] { jwtConfig.Audience },
                IgnoreTrailingSlashWhenValidatingAudience = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                RequireExpirationTime = true,
                ValidateLifetime = true
            };

            return tokenValidationParameters;
        }
    }
}
