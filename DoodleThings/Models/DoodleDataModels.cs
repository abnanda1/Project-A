using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoodleThings.Models
{
    public class Player
    {
        public Player() {  /* do nothing */ }
        public Player(string userId, string userName)
        {
            PlayerId = userId;
            Name = userName;
            State = PlayerState.Online;
            DrawerTotalPoints = 0;
            GuesserTotalPoints = 0;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string PlayerId { get; set; }
        public string Name { get; set; }
        public int DrawerTotalPoints { get; set; }
        public int GuesserTotalPoints { get; set; }
        public PlayerState State { get; set; }
    }

    public enum PlayerState
    {                
        Offline,
        Online,
        InQueue,
        InPlay
    }

    public class Challenge
    {
        public int ChallengeId { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }
        public int MaxPoints { get; set; }  // if hit within time limits...
        public int TimeLimitInSeconds { get; set; }
    }

    public class Session
    {
        public int SessionId { get; set; }
        public Player Drawer { get; set; }
        public Player Guesser { get; set; }
        public Challenge Challenge { get; set; }
        public int PointsEarned { get; set; }
        public SessionState State { get; set; }
        public DateTime Created { get; set; }
        public DateTime Started { get; set; }        
    }

    public enum SessionState
    {
        Created = 0,
        InPlay = 1,
        Won = 2,
        Lost = 3,
        Cancelled = 4,
        TimedOut = 5,        
    }
}
