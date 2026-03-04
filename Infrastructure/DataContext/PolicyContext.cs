using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext
{
    public class PolicyContext : DbContext
    {

     
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite($"Data Source={MainDBsPath.Path}Policy.db; ");

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // enforce unique label in Policies
            modelBuilder.Entity<Policies>()
                .HasIndex(p => p.Label)
                .IsUnique();
        }
        public DbSet<UserLabel> UserLabels { get; set; }
        public DbSet<Policies> Policies { get; set; }
    }
}
