
using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext
{
    public class AppConfigAndLoginContext : DbContext
    {



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite($"Data Source={MainDBsPath.Path}Application_ConfigurationDB.db;");

        }

        public DbSet<Configurations> Application_Configuration { get; set; }
        public DbSet<LoginModule> LoginModule { get; set; }
    }
}