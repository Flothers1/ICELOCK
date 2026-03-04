using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataContext
{
    public class CurrentProcessContext : DbContext
    {

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string dbFile = Path.Combine(MainDBsPath.DatabasesPath, "CurrentProcessDB.db");
        //    optionsBuilder.UseSqlite($@"Data Source={dbFile}; Password=$sZwty@sT*U7#b7E;");
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) // Make Connection With Database
        {
            optionsBuilder.UseSqlite($"Data Source = {MainDBsPath.Path}CurrentProcessDB.db; Password=$sZwty@sT*U7#b7E;");
        }
        public DbSet<Current_Process> Current_Process { get; set; }
    }
}
