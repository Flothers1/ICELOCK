using Infrastructure.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataContext
{
    public class UpdateSignsDBContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) // Make Connection With Database
        {
            optionsBuilder.UseSqlite($"Data Source = {MainDBsPath.Path}UpdateSignsDB.db; Password=$sZwty@sT*U7#b7E;");
        }


        public DbSet<UpdateSignsDB> UpdateSignsDB { get; set; }
        public DbSet<AddMalwareHashsFlag> AddMalwareHashsFlag { get; set; }
    }
}
