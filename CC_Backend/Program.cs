
using CC_Backend.Data;
using CC_Backend.Handlers;
using CC_Backend.Models;
using CC_Backend.Repositories.Friends;
using CC_Backend.Repositories.Stamps;
using CC_Backend.Repositories.User;
using CC_Backend.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CC_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register controllers
            builder.Services.AddControllers();

            // Add services to the container.
            builder.Services.AddAuthorization();
            string connectionString = builder.Configuration.GetConnectionString("NatureAI_DB");
            builder.Services.AddDbContext<NatureAIContext>(opt => opt.UseSqlServer(connectionString));

            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddIdentityCookies();
            builder.Services.AddAuthorizationBuilder();

            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<NatureAIContext>()
                .AddApiEndpoints();

            string apiKey = builder.Configuration.GetValue<string>("OpenAI:ApiKey");
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
            "manage/info"
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
            defaults: new { controller = "Logout", action = "Logout" }
);

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
