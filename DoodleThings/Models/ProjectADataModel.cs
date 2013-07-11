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
            ConnectionId = null;
            DrawerPoints = 0;
            GuesserPoints = 0;
            LockedOut = false;
            QuestionsAlreadyUsed = new HashSet<Question>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserInfoId { get; set; }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        public bool LockedOut { get; set; }
        public int DrawerPoints { get; set; }
        public int GuesserPoints { get; set; }

        public virtual ICollection<Question> QuestionsAlreadyUsed { get; set; }
    }

    public class Question
    {
        public int QuestionId { get; set; }
        public string HintText { get; set; }
        public string AnswerText { get; set; }
        public int MaxPoints { get; set; } // i.e. difficulty level

       // public virtual ICollection<UserInfo> UsersWhoHaveUsedThis { get; set; }
    }

    public class Game
    {
        public int GameId { get; set; }
        public string DrawerUserId { get; set; }
        public UserInfo DrawerUser { get; set; }
        public string GuesserUserId { get; set; }
        public UserInfo GuesserUser { get; set; }

        public int? QuestionId { get; set; }
        public Question Question { get; set; }
        public int? PointsEarned { get; set; }
        public GameState State { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime Created { get; set; }
    }

    public enum GameState
    {
        NotStarted = 0,
        InPlay = 1,
        TimedOut = 2,
        Completed = 3
    }
}