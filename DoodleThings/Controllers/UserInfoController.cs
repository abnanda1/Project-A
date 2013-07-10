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
         //Get all ready to play users except yourself and return one random userinfo
           var allUsers = ctx.UserInfos.Where( u => u.ReadyToPlay == true && u.UserName != userName).ToList();
           Random randNum = new Random();
           return allUsers[randNum.Next(allUsers.Count)];
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
            user.ReadyToPlay = readyToPlay;

            // Need to detach to avoid duplicate primary key exception when SaveChanges is called
            ctx.Entry(user).State = System.Data.Entity.EntityState.Detached;
            ctx.Entry(user).State = System.Data.Entity.EntityState.Modified;

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
