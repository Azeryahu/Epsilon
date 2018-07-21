using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsilon
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Knight> Knights { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Citizen> Citizens { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Alliance> Alliances { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=LEVIATHAN;Initial Catalog=RankDB;Integrated Security=True;Pooling=False");
        }
    }
}
