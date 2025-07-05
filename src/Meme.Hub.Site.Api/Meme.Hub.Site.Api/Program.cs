using Meme.Domain.Models;
using Meme.Hub.Site.Models;
using Meme.Hub.Site.Services;

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
                                      policy.WithOrigins("http://localhost:3000", "http://localhost:5173");
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

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
    }
}
