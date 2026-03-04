using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataContext
{
    public class SharedFilesContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) // Make Connection With Database
        {

            optionsBuilder.UseSqlite($"Data Source = {MainDBsPath.Path}SharedFiles.db;");
        }



        public DbSet<SharedFile> SharedFiles { get; set; }
    }
}
