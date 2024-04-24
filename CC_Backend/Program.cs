
using CC_Backend.Data;
using CC_Backend.Services;
using Microsoft.EntityFrameworkCore;

namespace CC_Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();
            string connectionString = builder.Configuration.GetConnectionString("NatureAI_DB");
            builder.Services.AddDbContext<NatureAIContext>(opt => opt.UseSqlServer(connectionString));



            string apiKey = builder.Configuration.GetValue<string>("OpenAI:ApiKey");
            builder.Services.AddSingleton<IOpenAIService>(x => new OpenAIService(apiKey));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();

            app.UseAuthorization();

           
          
            app.Run();
        }
    }
}
