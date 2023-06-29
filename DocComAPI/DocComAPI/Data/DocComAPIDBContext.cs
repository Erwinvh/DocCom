using DocComAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DocComAPI.Data
{
    public class DocComAPIDBContext: DbContext
    {

        public DocComAPIDBContext(DbContextOptions options) : base(options) {
            
        }

        public DbSet<user> Users { get; set; }
        public DbSet<document> Documents { get; set; }
        public DbSet<comment> Comments { get; set; }


    }
}
