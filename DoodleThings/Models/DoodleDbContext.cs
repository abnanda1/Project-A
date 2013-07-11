using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DoodleThings.Models
{
    public class DoddleDbContext : DbContext
    {
        public DoddleDbContext() : base("DefaultConnection") { }
        public DoddleDbContext(string nameOrConnStr) : base(nameOrConnStr) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<DoddleDbContext>(new CreateDatabaseIfNotExists<DoddleDbContext>());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Challenge> Challenges { get; set; }

    }
}