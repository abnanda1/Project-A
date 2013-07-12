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
        [HttpGet("{userInfoId}")]
        public UserInfo GetUserInfoFromId(string userInfoId)
        {
            return ctx.UserInfos.FirstOrDefault(u => u.UserInfoId == userInfoId);
        }

        // This is used to update the connection id of an existing user
        // PUT api/userinfo
       [HttpPut("{userInfoId}")]
        public IHttpActionResult UpdateConnectionId(string userName, string connectionId)
        {
            var userInfo = ctx.UserInfos.FirstOrDefault(u => u.UserName == userName);
            if (userInfo == null)
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            userInfo.ConnectionId = connectionId;
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
       
            //var user = ctx.UserInfos.FirstOrDefault(u => u.UserName == userName);
            //user.UserInfoId = connectionId;
        protected override void Dispose(bool disposing)
        {
            ctx.Dispose();
            base.Dispose(disposing);
        }
    }
}
