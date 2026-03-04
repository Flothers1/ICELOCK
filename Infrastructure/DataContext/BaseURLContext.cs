using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataContext
{
    public class BaseURLContext : DbContext
    {

        //public BaseURLContext()
        //{
        //}

        //public BaseURLContext(DbContextOptions<BaseURLContext> options)
        //    : base(options)
        //{
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    string dbFile = Path.Combine(MainDBsPath.DatabasesPath, "BaseURLDB.db");
        //    optionsBuilder.UseSqlite($@"Data Source={dbFile}; Password=$sZwty@sT*U7#b7E;");
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite($"Data Source={MainDBsPath.Path}BaseURLDB.db;");

        }


        public DbSet<CloudURL> CloudURL { get; set; }
    }
}
