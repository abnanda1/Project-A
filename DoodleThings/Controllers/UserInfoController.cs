using DoodleThings.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DoodleThings.Controllers
{
    [Authorize]
    [RoutePrefix("api/UserInfo")]
    public class UserInfoController : ApiController
    {
        private ProjectAContext ctx = new ProjectAContext();

        // This is used to create a new user
        // GET api/userinfo/1
        [HttpGet("{userId, userName}")]
        public UserInfo CreateNewLoggedOutUser(string userId, string userName)
        {
            try
            {
                UserInfo user = new UserInfo()
                {
                    UserInfoId = userId,
                    UserName = userName,
                    DrawerPoints = 0,
                    GuesserPoints = 0,
                    LockedOut = false,
                    State = UserState.LoggedOut
                };
                ctx.UserInfos.Add(user);
                ctx.SaveChanges();

                return user;
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
        }

        // This is called when 2nd player logs in to assign player to play with (first player will also call but will get null)
        // GET api/userinfo/2
        [HttpGet("{userName}")]
        public UserInfo GetRandomAvailablePlayer(string userName)
        {
            UserInfo user = null;
            //Get all ready to play users except yourself and return one random userinfo
            var allUsers = ctx.UserInfos.Where( u => u.State == UserState.ReadyToPlay && u.UserName != userName && u.LockedOut == false).ToList();
            if (allUsers != null)
            {
               Random randNum = new Random();
               user = allUsers[randNum.Next(allUsers.Count)];
            }
            return user;
        }

        // This is used by the admin user to list all users
        // GET api/userinfo/2
        public IEnumerable<UserInfo> GetAllPlayers()
        {
            return ctx.UserInfos.AsEnumerable();
        }

        // POST api/userinfo
        public void Post([FromBody]string value)
        {
                //DO NOTHING
        }

        // This is used by a user when they click the "ReadyToPlay" button
        // PUT api/userinfo/5
        [HttpPut("{userName}", RouteName = "UserInfo")]
        public IHttpActionResult UpdateReadyToPlay(string userName, [FromBody]bool readyToPlay)
        {
            if (!ModelState.IsValid)
            {
                return Message(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            var user = ctx.UserInfos.FirstOrDefault(u => u.UserName == userName);
            user.State = readyToPlay ? UserState.ReadyToPlay : UserState.LoggedIn;

            // shouldn't need to do this - I've updated the key to be non-database-generated
            //// Need to detach to avoid duplicate primary key exception when SaveChanges is called
            //ctx.Entry(user).State = System.Data.Entity.EntityState.Detached;
            //ctx.Entry(user).State = System.Data.Entity.EntityState.Modified;

            try
            {
                ctx.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.OK);
        }


        public void UpdateConnectionId(string userName, string connectionId)
        {

        }

        //This is used when the user logs out
        // PUT api/userinfo/6
        [HttpPut("{userName}", RouteName = "UserInfo")]
        public IHttpActionResult LogOutUser(string userName)
        {
            if (!ModelState.IsValid)
            {
                return Message(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            var user = ctx.UserInfos.FirstOrDefault(u => u.UserName == userName);
            user.State = UserState.LoggedOut;

            try
            {
                ctx.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // This is used by the admin user to prevent a user from playing
        // PUT api/userinfo/6
        [HttpPut("{userName}", RouteName = "UserInfo")]
        public IHttpActionResult LockOutUser(string userName, [FromBody]bool isLockedOut)
        {
            if (!ModelState.IsValid)
            {
                return Message(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            var user = ctx.UserInfos.FirstOrDefault(u => u.UserName == userName);
            user.LockedOut = isLockedOut;

            try
            {
                ctx.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return StatusCode(HttpStatusCode.OK);
        }
        
        protected override void Dispose(bool disposing)
        {
            ctx.Dispose();
            base.Dispose(disposing);
        }
    }
}
