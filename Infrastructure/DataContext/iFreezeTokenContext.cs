using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataContext
{
    public class iFreezeTokenContext : DbContext
    {

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string dbFile = Path.Combine(MainDBsPath.DatabasesPath, "iFreezeTokenDB.db");
        //    optionsBuilder.UseSqlite($@"Data Source={dbFile}; Password=$sZwty@sT*U7#b7E;");
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source = {MainDBsPath.Path}iFreezeTokenDB.db; Password=$sZwty@sT*U7#b7E;");
        }

        public DbSet<iFreezeToken> iFreezeToken { get; set; }

    }
}
