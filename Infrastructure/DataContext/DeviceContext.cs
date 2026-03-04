using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext
{
    public class DeviceContext : DbContext
    {

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string dbFile = Path.Combine(MainDBsPath.DatabasesPath, "DeviceActionsDB.db");
        //    optionsBuilder.UseSqlite($@"Data Source={dbFile}; Password=$sZwty@sT*U7#b7E;");
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite($"Data Source={MainDBsPath.Path}DeviceActionsDB.db; Password=$sZwty@sT*U7#b7E;");

        }

        public DbSet<DeviceActions> DeviceActions { get; set; }
    }
}
