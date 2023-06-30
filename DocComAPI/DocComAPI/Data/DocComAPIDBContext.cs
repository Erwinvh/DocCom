using DocComAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DocComAPI.Data
{
    public class DocComAPIDBContext: DbContext
    {

        public DocComAPIDBContext(DbContextOptions options) : base(options) {
            try
            {
                var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if(databaseCreator != null)
                {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();
                    if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



        }

        public DbSet<user> Users { get; set; }
        public DbSet<document> Documents { get; set; }
        public DbSet<comment> Comments { get; set; }


    }
}
