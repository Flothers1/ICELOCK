using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataContext
{
    public class FilesProtectionContext : DbContext
    {

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string dbFile = Path.Combine(MainDBsPath.DatabasesPath, "FilesProtectionDB.db");
        //    optionsBuilder.UseSqlite($@"Data Source={dbFile}; Password=$sZwty@sT*U7#b7E;");
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) // Make Connection With Database
        {
            optionsBuilder.UseSqlite($"Data Source = {MainDBsPath.Path}FilesProtectionDB.db; Password=$sZwty@sT*U7#b7E;");
        }


        public DbSet<FilesProtections> FilesProtection { get; set; }
    }
}
