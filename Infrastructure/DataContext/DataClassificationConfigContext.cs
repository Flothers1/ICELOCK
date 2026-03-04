using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext
{
    public class DataClassificationConfigContext : DbContext
    {

     
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite($"Data Source={MainDBsPath.Path}DataClassificationConfig.db;");

        }

        public DbSet<DataClassificationSettings> DataClassificationSettings { get; set; }
    }
}
