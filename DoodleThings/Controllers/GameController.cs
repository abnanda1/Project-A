using DoodleThings.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DoodleThings.Controllers
{
    [Authorize]
    [RoutePrefix("api/Game")]
    public class GameController : ApiController
    {
        private ProjectAContext ctx = new ProjectAContext();
        
        // GET api/Game/kirthik
        [HttpGet("{id}", RouteName = "Game")]
        public Game GetGame(string userId)
        {
            return ctx.Games.FirstOrDefault(x => x.DrawerUserId == userId || x.GuesserUserId == userId);
        }

        // POST api/Game
        [HttpPost("")]
        public HttpResponseMessage PostGame(string drawerUserId, string guesserUserId)
        {
            Game myGame = new Game();
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var userId = User.Identity.GetUserId();
            //If userId is not equal to either drawerUserId or guesserUserId then unauthorized else add a game with drawerUserId and guesserUserId
            if (userId.Equals(drawerUserId) || userId.Equals(guesserUserId))
            {
                UserInfo drawerUserInfo =  ctx.UserInfos.FirstOrDefault(u1 => u1.UserInfoId == drawerUserId);
                UserInfo guesserUserInfo = ctx.UserInfos.FirstOrDefault(u2 => u2.UserInfoId == guesserUserId);
                myGame.DrawerUser = drawerUserInfo;
                myGame.GuesserUser = guesserUserInfo;

                var questionsAlreadyUsed = drawerUserInfo.QuestionsAlreadyUsed.Union(guesserUserInfo.QuestionsAlreadyUsed);
                List<Question> qsToUse = ctx.Questions.Where(q => (!questionsAlreadyUsed.Contains(q))).ToList();

                // if list is empty
                if (qsToUse == null || qsToUse.Count == 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ModelState);
                }

                //Get a random question from qsToUse List
                Random randNum = new Random();
                Question currentQ;
                currentQ = qsToUse[randNum.Next(qsToUse.Count)];

                //Add the question to myGame
                myGame.Question = currentQ;
                
                // shouldn't have to do both
                //myGame.QuestionId = currentQ.QuestionId;

                myGame.State = GameState.InPlay;
                myGame.StartedAt = DateTime.Now;

                //Also update questions already asked lists
                drawerUserInfo.QuestionsAlreadyUsed.Add(currentQ);
                guesserUserInfo.QuestionsAlreadyUsed.Add(currentQ);

                try
                {
                    ctx.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ModelState);
            }

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(Url.Link("Game", new { id = myGame.GameId }));
            return response;
        }

        // PUT api/Game/5
        [HttpPut("{id}")]
        public IHttpActionResult UpdateGame(int id, Game myGame, [FromBody]int pointsEarned)
        {
            if (!ModelState.IsValid)
            {
                return Message(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            if (id != myGame.GameId)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }

            if (!String.Equals(ctx.Entry(myGame).Entity.DrawerUserId, User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase) ||
                !String.Equals(ctx.Entry(myGame).Entity.GuesserUserId, User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase))
            {
                // Trying to modify a record that does not belong to the user
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            // shouldn't need to do this - I've updated the key to be non-database-generated
            //// Need to detach to avoid duplicate primary key exception when SaveChanges is called
            //ctx.Entry(myGame).State = EntityState.Detached;
            //ctx.Entry(myGame).State = EntityState.Modified;

            //if mygame state is finished then we need to update points earned for both users
            if (myGame.State == GameState.Completed)
            {
                myGame.DrawerUser.DrawerPoints += pointsEarned;
                myGame.GuesserUser.GuesserPoints += pointsEarned;
            }

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
