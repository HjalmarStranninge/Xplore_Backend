using CC_Backend.Data;
using CC_Backend.Handlers;
using CC_Backend.Models;
using CC_Backend.Repositories.Friends;
using CC_Backend.Repositories.Stamps;
using CC_Backend.Repositories.User;
using CC_Backend.Services;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Authentication.OAuth;
using CC_Backend.Repositories.LikeRepo;
using CC_Backend.Repositories.GeodataRepo;

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
            services.AddControllers();


            // Add services to the container.
            services.AddAuthorization();


            // Database setup
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            services.AddDbContext<NatureAIContext>(opt => 
            opt.UseSqlServer(connectionString));


            // Add Identity services
            services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddIdentityCookies();
            services.AddAuthorizationBuilder();

            services.AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<NatureAIContext>()
                .AddApiEndpoints();


            // CORS configuration
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:7231", "http://127.0.0.1:5500", "https://johantran02.github.io")
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });


            // Google setup
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
                {
                    googleOptions.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENTID");
                    googleOptions.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENTSECRET");
                });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            // JWT Authentication Configuration
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRETKEY");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });


            // Set up dependency injection
            string apiKey = Environment.GetEnvironmentVariable("OPENAI_KEY");
            services.AddSingleton<IOpenAIService>(x => new OpenAIService(apiKey));
            services.AddSingleton<MimeKit.MimeMessage>();
            services.AddScoped<IStampsRepo, StampsRepo>();
            services.AddScoped<IFriendsRepo, FriendsRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IStampHandler, StampHandler>();
            services.AddScoped<ICommentRepo, CommentRepo>();
            services.AddScoped<ILikeRepo, LikeRepo>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IGeodataRepo, GeodataRepo>();
            services.AddScoped<IJwtAuthManager>(provider =>
            {
                var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
                return new JwtAuthManager(userManager, secretKey);
            });
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ILogger, Logger<AccountService>>();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
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
                        "register",
                        "login"
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
              
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllerRoute(
            name: "logout",
            pattern: "logout",
            defaults: new { controller = "Logout", action = "Logout" });

            app.UseHttpsRedirection();
            app.UseCors("AllowSpecificOrigins");


            // Middleware to handle preflight requests
            app.Use(async (context, next) =>
            {
                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "https://johantran02.github.io");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
                    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    context.Response.StatusCode = 204; // No Content
                    return;
                }

                await next.Invoke();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}