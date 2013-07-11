using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DoodleThings.Models
{
    public class SessionViewModel
    {
        public SessionViewModel() { }

        public SessionViewModel(Session item)
        {
            SessionId = item.SessionId;
            //Title = item.Title;
            //IsDone = item.IsDone;
            //TodoListId = item.TodoListId;
        }

        [Key]
        public int SessionId { get; set; }

        //[Required]
        //public string Title { get; set; }

        //public bool IsDone { get; set; }

        //public int TodoListId { get; set; }

        public Session ToEntity()
        {
            return new Session
            {
                SessionId = SessionId,
                Created = DateTime.Now,
                State = SessionState.Created,
                
            };
        }
    }
}
