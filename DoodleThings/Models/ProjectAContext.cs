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
            Database.SetInitializer<ProjectAContext>(new SeedingDatabaseInitializer());

            modelBuilder.Entity<Game>().HasRequired<UserInfo>(g => g.DrawerUser).WithMany().HasForeignKey(g => g.DrawerUserId).WillCascadeOnDelete(false);
            modelBuilder.Entity<Game>().HasRequired<UserInfo>(g => g.GuesserUser).WithMany().HasForeignKey(g => g.GuesserUserId).WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Question> Questions { get; set;}
    }

    public class SeedingDatabaseInitializer : DropCreateDatabaseIfModelChanges<ProjectAContext>
    {
        protected override void Seed(ProjectAContext context)
        {
            var user1 = new UserInfo {UserInfoId="TestUser1", UserName="TestUser1", ConnectionId = null, DrawerPoints = 0, GuesserPoints = 0, LockedOut = false, QuestionsAlreadyUsed = new HashSet<Question>() };
            var user2 = new UserInfo {UserInfoId="TestUser2", UserName="TestUser2", ConnectionId = null, DrawerPoints = 0, GuesserPoints = 0, LockedOut = false, QuestionsAlreadyUsed = new HashSet<Question>() };
            var user3 = new UserInfo { UserInfoId = "TestUser3", UserName = "TestUser3", ConnectionId = null, DrawerPoints = 0, GuesserPoints = 0, LockedOut = false, QuestionsAlreadyUsed = new HashSet<Question>() };

            var q1 = new Question { HintText = "Test Hint 1", AnswerText = "Answer1", MaxPoints = 100, UsersWhoHaveUsedThis = new HashSet<UserInfo> { user1 } };
            var q2 = new Question { HintText = "Test Hint 2", AnswerText = "Answer2", MaxPoints = 50, UsersWhoHaveUsedThis = new HashSet<UserInfo> { user2 } };
            var q3 = new Question { HintText = "Test Hint 3", AnswerText = "Answer2", MaxPoints = 10, UsersWhoHaveUsedThis = new HashSet<UserInfo> { user3 } };

            user1.QuestionsAlreadyUsed.Add(q1);
            user2.QuestionsAlreadyUsed.Add(q2);
            user3.QuestionsAlreadyUsed.Add(q3);

            // add data into context and save to db
            if (context.UserInfos.FirstOrDefault(ui => ui.UserInfoId.StartsWith("TestUser")) == null)
            {

                List<UserInfo> users = new List<UserInfo> { user1, user2, user3 };

                foreach (UserInfo ui in users)
                {
                    context.UserInfos.Add(ui);
                }
            }

            if (context.Questions.FirstOrDefault(q => q.HintText.StartsWith("Test Hint")) == null)
            {
                List<Question> qs = new List<Question> { q1, q2, q3 };

                foreach (var q in qs)
                {
                    context.Questions.Add(q);
                }
            }

            context.SaveChanges();
        }
    }
}