using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using DoodleThings.Models;

namespace DoodleThings.Controllers
{

    [Authorize]
    [RoutePrefix("api/doodlesession")]
    public class DoodleSessionController : ApiController
    {
        private DoddleDbContext db = new DoddleDbContext();

        // ***************************************************
        // Required APIs
        // ***************************************************


        // Get All Available Sessions:  Sessions with Created state        
        [HttpGet("")]
        public IEnumerable<SessionViewModel> GetAvailableSessions()
        {
            //string cachedUserId = User.Identity.GetUserId();
            return db.Sessions.Include("Sessions")
                .Where(s => s.State == SessionState.Created)
                .OrderByDescending(u => u.Created)
                .AsEnumerable()
                .Select(session => new SessionViewModel(session));
        }

        // Get Given session:  Session with an ID
        [HttpGet("{id}", RouteName = "doodlesession")]
        public IHttpActionResult GetSession(int id)
        {
            var session = db.Sessions.Find(id);
            if (session == null)
            {
                return StatusCode(HttpStatusCode.NotFound);
            }

            //if (!String.Equals(session.UserId, User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase))
            //{
            //    // Trying to modify a record that does not belong to the user
            //    return StatusCode(HttpStatusCode.Unauthorized);
            //}

            return Content(HttpStatusCode.OK, new SessionViewModel(session));
        }
        
        // Create Session:  Creates a new session with given available player.  Sets both drawer and guesser to the player. sets state to Created
        [HttpPost("")]
        public HttpResponseMessage CreateSession(SessionViewModel sessionDto)
        {


            return null;
        }

        // Add Player To Session:  Adds a new player to the session. If session is not in Created state, throw error 
        // TimeOut session:  Move to corresponding state
        // Cancel session:  Move to corresponding state        
        // End session with given points:  0:  no win.. otherwise totalpoints of each player is decreased.
        // All above are PUT calls..

        [HttpPut("{id}")]
        public IHttpActionResult UpdateSession(int id, SessionViewModel sessionDto)
        {
            return null;
        }


    }
}
