using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoodleThings.Models
{
    public class UserInfo
    {
        public UserInfo() {  /* do nothing */ }
        public UserInfo(string userId, string userName)
        {
            UserInfoId = userId;
            UserName = userName;
            LoggedIn = true;
            DrawerPoints = 0;
            GuesserPoints = 0;
            LockedOut = false;
            ReadyToPlay = false;
            QuestionsAlreadyUsed = new HashSet<Question>();
        }

        [Key]
        public string UserInfoId { get; set; }
        public string UserName { get; set; }
        public bool LoggedIn { get; set; }
        public bool LockedOut { get; set; }
        public int DrawerPoints { get; set; }
        public int GuesserPoints { get; set; }
        public bool ReadyToPlay { get; set; }

        public virtual ICollection<Question> QuestionsAlreadyUsed { get; set; }
    }

    public class Question
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public int MaxPoints { get; set; } // i.e. difficulty level

        public virtual ICollection<UserInfo> UsersWhoHaveUsedThis { get; set; }
    }

    public class Game
    {
        public int GameId { get; set; }
        public string User1Id { get; set; }
        public UserInfo User1 { get; set; }
        public string User2Id { get; set; }
        public UserInfo User2 { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public int? PointsEarned { get; set; }
        public State State { get; set; }
        public DateTime? StartedAt { get; set; }
    }

    public enum State
    {
        NotStarted = 0,
        InPlay = 1,
        TimedOut = 2,
        Completed = 3
    }
}