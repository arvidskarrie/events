using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace EventDatabaseBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddEnvironmentVariables();

            builder.Services.AddDbContext<EventDb>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("EventDbConnection"))
            );
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("OnlyGithubAllowed", builderCors => 
                {
                    builderCors.WithOrigins("https://arvidskarrie.github.io")
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                });
            });

            var app = builder.Build();
            app.UseCors("OnlyGithubAllowed");

            EventEndpoints.InitiateEndpoints(app.MapGroup("/eventitems"));

            app.Run();

            
        }
    }
}