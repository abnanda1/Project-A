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
 //   [Authorize]
    [RoutePrefix("api/Game")]
    public class GameController : ApiController
    {
        private ProjectAContext ctx = new ProjectAContext();

        //Get all games that are NotStarted
        [HttpGet("")]
        public IEnumerable<Game> GetAllAvailableGames()
        {
            return ctx.Games.Where(g => g.State == GameState.NotStarted);
        }

        // This is used to find a game that the user is currently playing
        [HttpGet("{userId}")]
        public Game GetCurrentGameForPlayer(string userId)
        {
            return ctx.Games.FirstOrDefault(x => (x.DrawerUserId == userId || x.GuesserUserId == userId) && (x.State == GameState.InPlay || x.State == GameState.NotStarted) );
        }

        [HttpPost]
        public IHttpActionResult AssignRandomAvailableGame(string userId)
        {
            Game myGame;
            HttpResponseMessage response;
            var games = ctx.Games.Include("DrawerUser").Where(g => g.State == GameState.NotStarted).ToList();

            //If there aren't any games that are not started then create a new one
            if (games == null || games.Count == 0)
            {
                myGame = new Game()
                {
                    Created = DateTime.Now,
                    DrawerUserId = userId,
                    GuesserUserId = userId,
                    PointsEarned = 0,
                    State = GameState.NotStarted,
                };

                ctx.Games.Add(myGame);
            }
            else
            {
                //TO DO: Think about scenatio where there are no questions.

                //var questionsAlreadyUsed = myGame.DrawerUser.QuestionsAlreadyUsed.Union(myGame.GuesserUser.QuestionsAlreadyUsed);
                //List<Question> qsToUse = ctx.Questions.Where(q => (!questionsAlreadyUsed.Contains(q))).ToList();

                //if (qsToUse == null || qsToUse.Count == 0)
                //{
                //    return null;
                //}

                Random rand = new Random();
                myGame = games[rand.Next(games.Count)];

                if (myGame.DrawerUserId == userId && myGame.GuesserUserId == userId)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, myGame);
                    return Message(response);
                }
                //Change guesser of the game to current user. Keep drawer as is. Change game state to InPlay. Change Started time. Add a question
                int randomInt = rand.Next(2);
                if (randomInt == 0)
                {
                    myGame.GuesserUserId = userId;
                }
                else
                {
                    myGame.DrawerUserId = userId;
                }
                myGame.State = GameState.InPlay;
                myGame.StartedAt = DateTime.Now;


                //Get a random question from qsToUse List
                Random randNum = new Random();
                Question currentQ;

                List<Question> qsToUse = ctx.Questions.ToList();
                currentQ = qsToUse[randNum.Next(qsToUse.Count)];

                //Add the question to myGame
                myGame.Question = currentQ;
            }
            ctx.SaveChanges();

            response = Request.CreateResponse(HttpStatusCode.Created, myGame);
          //  response.Headers.Location = new Uri(Url.Link("Game", new { id = myGame.GameId }));
            return Message(response);
        }

        // This is called when the guesser successfully guesses before the game times out
        //[HttpPut("api/Game/GameSuccessfullyGuessed/{gameId}/{pointsEarned}")]
        [HttpPost]
        public IHttpActionResult GameSuccessfullyGuessed([FromUri]int gameId, [FromUri]int pointsEarned)
        {
            if (!ModelState.IsValid)
            {
                return Message(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            Game myGame = ctx.Games.FirstOrDefault(g => g.GameId == gameId);

            if (myGame == null)
            {
                return Message(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "GameId cannot be null"));
            }

            //mygame state is finished then we need to update points earned for both users
            myGame.State = GameState.Completed;
            myGame.DrawerUser.DrawerPoints += pointsEarned;
            myGame.GuesserUser.GuesserPoints += pointsEarned;
            myGame.PointsEarned = pointsEarned;

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

        // This is called when the guesser successfully guesses before the game times out
        [HttpPut("api/Game/GameTimedOut/{gameId}")]
        public IHttpActionResult GameTimedOut(int gameId)
        {
            if (!ModelState.IsValid)
            {
                return Message(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            Game myGame = ctx.Games.FirstOrDefault(g => g.GameId == gameId);

            if (myGame == null)
            {
                return Message(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "GameId cannot be null"));
            }

            //mygame state is finished then we need to update points earned for both users
            myGame.State = GameState.TimedOut;

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
