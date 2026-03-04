using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataContext
{
    public class i_Freeze_WindowContext : DbContext
    {

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string dbFile = Path.Combine(MainDBsPath.DatabasesPath, "iFreeze_WindowDB.db");
        //    optionsBuilder.UseSqlite($@"Data Source={dbFile}; Password=$sZwty@sT*U7#b7E;");
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) // Make Connection With Database
        {
            optionsBuilder.UseSqlite($"Data Source = {MainDBsPath.Path}iFreeze_WindowDB.db; Password=$sZwty@sT*U7#b7E;");
        }
        public DbSet<i_Freeze_Window> i_Freeze_Window { get; set; }
    }
}
