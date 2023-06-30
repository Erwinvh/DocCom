using DocComAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace DocComAPI
{
    public class Program
    {

        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //builder.Services.AddDbContext<DocComAPIDBContext>(options => options.UseInMemoryDatabase("DocComDB"));

            //builder.Services.AddDbContext<DocComAPIDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            //builder.Services.AddDbContext<DocComAPIDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DocComAPIConnection")));
            var dbHost = Environment.GetEnvironmentVariable("DB_Host");
            var dbName = Environment.GetEnvironmentVariable("DB_NAME");
            var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
            var connectionString = $"Data source={dbHost};Initial Catalog={dbName};User ID=SA;Password={dbPassword};TrustServerCertificate=true";

            builder.Services.AddDbContext<DocComAPIDBContext>(options => options.UseSqlServer(connectionString, builder => builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}









