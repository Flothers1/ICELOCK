using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext
{
    public class ActivateDLPContext : DbContext
    {

     
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite($"Data Source={MainDBsPath.Path}ActivateDLP.db;");

        }

        public DbSet<ActivateDLPEntity> ActivateDLPEntity { get; set; }
    }
}
