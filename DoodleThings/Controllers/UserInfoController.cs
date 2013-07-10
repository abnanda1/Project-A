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
    public class UserInfoController : ApiController
    {
        private ProjectAContext ctx = new ProjectAContext();
        
        // GET api/userinfo/1
        public UserInfo GetRandomAvailablePlayer(string userName)
        {
            UserInfo user = null;
            //Get all ready to play users except yourself and return one random userinfo
            var allUsers = ctx.UserInfos.Where( u => u.State == UserState.ReadyToPlay && u.UserName != userName).ToList();
            if (allUsers != null)
            {
               Random randNum = new Random();
               user = allUsers[randNum.Next(allUsers.Count)];
            }
            return user;
        }

        // POST api/userinfo
        public void Post([FromBody]string value)
        {
                //DO NOTHING
        }

        // PUT api/userinfo/5
        [HttpPut("{userName}", RouteName = "UserInfo")]
        public IHttpActionResult UpdatePlayingState(string userName, [FromBody]bool readyToPlay)
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

        // PUT api/userinfo/5
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

        protected override void Dispose(bool disposing)
        {
            ctx.Dispose();
            base.Dispose(disposing);
        }
    }
}
