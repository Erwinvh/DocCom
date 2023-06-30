using DocComAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace DocComAPI
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            //builder.Services.AddDbContext<DocComAPIDBContext>(options => options.UseInMemoryDatabase("DocComDB"));

            //builder.Services.AddDbContext<DocComAPIDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
            //builder.Services.AddDbContext<DocComAPIDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DocComAPIConnection")));

            services.AddDbContext<DbContext>(options =>
            {
                var server = Configuration["ServerName"];
                var port = "1433";
                var database = Configuration["db"];
                var user = Configuration["SA"];
                var password = Configuration["DocComAPI1!"];

                options.UseSqlServer(
                    $"Server={server},{port};Initial Catalog={database};User ID={user};Password={password}",
                    sqlServer => sqlServer.MigrationsAssembly("DocComAPI"));
            });


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }
}
