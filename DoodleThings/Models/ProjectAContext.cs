using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DoodleThings.Models
{
    public class ProjectAContext : DbContext
    {
        public ProjectAContext()
            : base("DefaultConnection")
        {
        }

        public ProjectAContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<ProjectAContext>(new CreateDatabaseIfNotExists<ProjectAContext>());

            modelBuilder.Entity<Game>().HasRequired<UserInfo>(g => g.DrawerUser).WithMany().HasForeignKey(g => g.DrawerUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Game>().HasRequired<UserInfo>(g => g.GuesserUser).WithMany().HasForeignKey(g => g.GuesserUserId).WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Question> Questions { get; set;}
    }
}