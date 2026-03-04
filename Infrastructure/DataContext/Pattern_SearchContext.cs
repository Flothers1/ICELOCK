using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext
{
    public class Pattern_SearchContext : DbContext
    {

     
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite($"Data Source={MainDBsPath.Path}Pattern_Search.db; ");

        }

        public DbSet<file_rules> file_rules { get; set; }
        public DbSet<PatternEntity> Patterns { get; set; }
    }
}
