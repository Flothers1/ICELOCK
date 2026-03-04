

using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.DataContext
{
    public class Apps_VersionsContext : DbContext
    {

        //public Apps_VersionsContext()
        //{
        //}

        //public Apps_VersionsContext(DbContextOptions<Apps_VersionsContext> options)
        //    : base(options)
        //{
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string dbFile = Path.Combine(MainDBsPath.DatabasesPath, "AppsVersionsDB.db");
        //    optionsBuilder.UseSqlite($@"Data Source={dbFile}; Password=$sZwty@sT*U7#b7E;");
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite($"Data Source={MainDBsPath.Path}AppsVersionsDB.db; Password=$sZwty@sT*U7#b7E;");

        }
        public DbSet<Apps_Versions> Apps_Versions { get; set; }
    }
}
