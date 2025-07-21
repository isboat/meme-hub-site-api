using Meme.Domain.Models;
using Meme.Hub.Site.Common;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services;
using Meme.Hub.Site.Services.Repository;
using Meme.Hub.Site.Services.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Meme.Hub.Site.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<MongoSettings>(
                builder.Configuration.GetSection("MongoSettings"));

            builder.Services.Configure<S3Settings>(
                builder.Configuration.GetSection("S3Settings"));

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("https://delightful-beach-04563c903.2.azurestaticapps.net","http://localhost:3000", "http://localhost:5173");
                                      policy.AllowCredentials();
                                      policy.AllowAnyMethod();
                                      policy.AllowAnyHeader();
                                  });
            });

            builder.Services.AddAutoMapper(typeof(MappingProfile));


            // builder.Services.AddResponseCaching();

            // Add services to the container.

            builder.Services.AddSingleton<ICacheService, CosmosDBCacheService>();
            builder.Services.AddSingleton<IDatabaseService, CosmosDBService>();
            builder.Services.AddSingleton<IStorageService, S3StorageService>();
            builder.Services.AddSingleton<DataStore>();
            builder.Services.AddSingleton<IAuthService,AuthService>(); // Register AuthService
            builder.Services.AddSingleton<ICosmosDBRepository, CosmosDBRepository>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddSingleton<IProfileService, ProfileService>();
            builder.Services.AddSingleton<IMemeTokenService, MemeTokenService>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            RegisterJwtAuth(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static void RegisterJwtAuth(WebApplicationBuilder builder)
        {
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Privy Issuer URL is not configured.");
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Privy Audience URL is not configured.");
            var jwtSigningKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Privy Audience URL is not configured.");

            var isAuthenticationDisabled = false;
            
            if (!isAuthenticationDisabled)
            {
                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy(SiteAuthorization.RequiredPolicy, policy =>
                        policy.RequireAuthenticatedUser());
                });

                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>

                        {
                            options.SaveToken = true;
                            options.TokenValidationParameters = new()
                            {
                                RequireExpirationTime = true,
                                RequireSignedTokens = true,
                                ValidateAudience = true,
                                ValidateIssuer = true,
                                ValidateLifetime = true,

                                // Allow for some drift in server time
                                // (a lower value is better; we recommend two minutes or less)
                                ClockSkew = TimeSpan.FromMinutes(5),

                                ValidIssuer = jwtIssuer,
                                ValidAudience = jwtAudience,
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSigningKey))
                            };
                        });
            }
            else // authenticate anyone
            {
                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddScheme<AuthenticationSchemeOptions, EmptyAuthHandler>
                        (JwtBearerDefaults.AuthenticationScheme, opts => { });
            }
        }
    }
}
