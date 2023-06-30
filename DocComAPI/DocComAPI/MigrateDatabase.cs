using Microsoft.EntityFrameworkCore;

namespace DocComAPI
{
    public static class MigrateDatabase
    {

        public static IHost Migrate<T>(this IHost webHost) where T : DbContext {

            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetService<T>();
                    db.Database.Migrate();
                }catch (Exception ex)
                {
                    var logger = services.GetRequiredService<Logger<Program>>();
                    logger.LogError(ex, "An arror occured while migrating database");
                }
            }
        return webHost;
        
        }


    }
}
