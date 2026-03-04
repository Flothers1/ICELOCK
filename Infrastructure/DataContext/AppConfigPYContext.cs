
using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.DataContext
{
    public class AppConfigPYContext : DbContext
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string dbFile = Path.Combine(MainDBsPath.DatabasesPath, "Application_ConfigurationPY.db");
        //    optionsBuilder.UseSqlite($@"Data Source={dbFile}; Password=$sZwty@sT*U7#b7E;");
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) // Make Connection With Database
        {
            optionsBuilder.UseSqlite($"Data Source = {MainDBsPath.Path}Application_ConfigurationPY.db; Password=$sZwty@sT*U7#b7E;");
        }

        public DbSet<DevicePY> Application_Configuration { get; set; }

    }


    
}
