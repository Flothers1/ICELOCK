using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext
{
    public class DLPContext : DbContext
    {

     
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite($"Data Source={MainDBsPath.Path}DLP.db; Password=$sZwty@sT*U7#b7E;");

        }

        public DbSet<LabeledFile> LabeledFile { get; set; }
    }
}
