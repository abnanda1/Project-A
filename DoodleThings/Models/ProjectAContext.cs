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

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Question> Questions { get; set;}
    }
}