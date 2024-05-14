using CC_Backend.Data;
using CC_Backend.Handlers;
using CC_Backend.Models;
using CC_Backend.Repositories.Friends;
using CC_Backend.Repositories.Stamps;
using CC_Backend.Repositories.User;
using CC_Backend.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CC_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            DotNetEnv.Env.Load();

            var services = builder.Services;
            var configuration = builder.Configuration;

            // Register controllers
            builder.Services.AddControllers();

            // Add services to the container.

            builder.Services.AddAuthorization();
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            builder.Services.AddDbContext<NatureAIContext>(opt => opt.UseSqlServer(connectionString));

            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddIdentityCookies();
            builder.Services.AddAuthorizationBuilder();

            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<NatureAIContext>()
                .AddApiEndpoints();

            var AllowLocalhostOrigin = "_allowLocalhostOrigin";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    policy =>
                    {
                        policy.WithOrigins("http://127.0.0.1:5500/")
                         .AllowAnyHeader()
                         .AllowAnyMethod()
                         .AllowCredentials();
                    });
            });

            // Set up Google SSO.

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENTID");
                googleOptions.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENTSECRET");
            });

            // Dependency injection:

            string apiKey = Environment.GetEnvironmentVariable("OPENAI_KEY");
            builder.Services.AddSingleton<IOpenAIService>(x => new OpenAIService(apiKey));
            builder.Services.AddScoped<IStampsRepo, StampsRepo>();
            builder.Services.AddScoped<IFriendsRepo, FriendsRepo>();
            builder.Services.AddScoped<IUserRepo, UserRepo>();
            builder.Services.AddScoped<IStampHandler, StampHandler>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddSingleton<MimeKit.MimeMessage>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var routeTemplate = apiDesc.RelativePath;
                    var endpointsToHide = new List<string>
                    {
                        "refresh",
                        "confirmEmail",
                        "resendConfirmationEmail",
                        "forgotPassword",
                        "resetPassword",
                        "manage/2fa",
                        "manage/info",
                        "manage/info",
                        "register"
                    };
                    foreach (var endpoint in endpointsToHide)
                    {
                        if (routeTemplate == endpoint)
                            return false;
                    }
                    return true;
                });
            });


            var app = builder.Build();

            app.MapIdentityApi<ApplicationUser>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllerRoute(
            name: "logout",
            pattern: "logout",
            defaults: new { controller = "Logout", action = "Logout" });


            app.UseHttpsRedirection();

            app.UseCors(AllowLocalhostOrigin);

            app.UseAuthorization();

            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}