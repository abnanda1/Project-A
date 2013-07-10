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
            return ctx.Games.FirstOrDefault(x => x.User1Id == userId || x.User2Id == userId);
        }

        // POST api/Game
        [HttpPost("")]
        public HttpResponseMessage PostGame(string user1, string user2)
        {
            Game myGame = new Game();
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var userId = User.Identity.GetUserId();
            //If userId is not equal to either user1 or user2 then unauthorized else add a game with user1 and user2
            if (userId.Equals(user1) || userId.Equals(user2))
            {
                UserInfo userInfo1 =  ctx.UserInfos.FirstOrDefault(u1 => u1.UserName == user1); 
                UserInfo userInfo2 =  ctx.UserInfos.FirstOrDefault(u2 => u2.UserName == user2);
                myGame.User1 = userInfo1;
                myGame.User2 = userInfo2;

                var questions = ctx.Questions.AsEnumerable();
                List<Question> qsToUse = new List<Question>();

                foreach(Question q in questions)
                {
                    //If Question q is used already by user1 or user2, don't add it to questions list
                    if (userInfo1.QuestionsAlreadyUsed.Contains(q) || userInfo2.QuestionsAlreadyUsed.Contains(q))
                    {
                        //DO NOTHING
                    }
                    else
                    {
                        qsToUse.Add(q);
                    }
                }

                //Get a random question from qsToUse List
                Random randNum = new Random();
                Question currentQ;
                currentQ = qsToUse[randNum.Next(qsToUse.Count)];

                //Add the question to myGame
                myGame.Question = currentQ;
                myGame.QuestionId = currentQ.QuestionId;

                myGame.State = State.InPlay;
                myGame.StartedAt = DateTime.Now;

                //Also update user1 and user2's questions already asked list
                userInfo1.QuestionsAlreadyUsed.Add(currentQ);
                userInfo2.QuestionsAlreadyUsed.Add(currentQ);

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
        public IHttpActionResult UpdateGame(int id, Game myGame)
        {
            if (!ModelState.IsValid)
            {
                return Message(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            if (id != myGame.GameId)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }

            if (!String.Equals(ctx.Entry(myGame).Entity.User1Id, User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase) ||
                !String.Equals(ctx.Entry(myGame).Entity.User2Id, User.Identity.GetUserId(), StringComparison.OrdinalIgnoreCase))
            {
                // Trying to modify a record that does not belong to the user
                return StatusCode(HttpStatusCode.Unauthorized);
            }

            // Need to detach to avoid duplicate primary key exception when SaveChanges is called
            ctx.Entry(myGame).State = EntityState.Detached;
            ctx.Entry(myGame).State = EntityState.Modified;

            //TO DO: if mygame state is finished then we need to update pointed earned for both users
            if (myGame.State == State.Completed)
            {
                //TO DO: We need to know who the guesser or drawer is; so we can assign guesser/drawer points appropriately to users
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
