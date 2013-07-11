using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DoodleThings.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace DoodleThings.Controllers
{
    [Authorize]
    [RoutePrefix("api/Questions")]
    public class QuestionsController : ApiController
    {
        private ProjectAContext ctx = new ProjectAContext();

        //// GET api/Questions/1
        //[HttpGet("")]
        //public IEnumerable<Question> GetQuestionsForUser()
        //{

        //    string cachedUserId = User.Identity.GetUserId();
        //    Models.UserInfo userInfo = ctx.UserInfos.FirstOrDefault(ui => ui.UserInfoId == cachedUserId);
        //    return ctx.Questions.Where(q => !q.UsersWhoHaveUsedThis.Contains(userInfo));
        //}

        // GET api/Questions
        [HttpGet("")]
        public IEnumerable<Question> GetAllQuestions()
        {
            return ctx.Questions.AsEnumerable();
        }

        // GET api/Questions
        [HttpGet("{id}")]
        public Question GetQuestionByFromId(int questionId)
        {
            return ctx.Questions.FirstOrDefault(q => q.QuestionId == questionId);
        }


        // PUT api/question/5
        [HttpPut("{id}", RouteName = "Question")]
        public IHttpActionResult UpdateQuestion(int id, Question question)
        {
            if (!ModelState.IsValid)
            {
                return Message(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            if (id != question.QuestionId)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }

            // shouldn't need to do this - I've updated the key to be non-database-generated
            //// Need to detach to avoid duplicate primary key exception when SaveChanges is called
            //ctx.Entry(question).State = EntityState.Detached;
            //ctx.Entry(question).State = EntityState.Modified;

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

        // POST api/question
        [HttpPost("")]
        public IHttpActionResult CreateQuestion([FromBody]string hintText, [FromBody]string answerText, [FromBody]int maxPoints)
        {
            if (!ModelState.IsValid)
            {
                return Message(Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState));
            }

            var q = new Question()
            {
                HintText = hintText,
                AnswerText = answerText,
                MaxPoints = maxPoints,
            };
            ctx.Questions.Add(q);
            ctx.SaveChanges();
            

            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, q);
            response.Headers.Location = new Uri(Url.Link("Question", new { id = q.QuestionId }));
            return Message(response);
        }

        // DELETE api/question/5
        [HttpDelete("{id}")]
        public IHttpActionResult DeleteQuestion(int id)
        {
            Question question = ctx.Questions.Find(id);
            if (question == null)
            {
                return StatusCode(HttpStatusCode.NotFound);
            }

            ctx.Questions.Remove(question);

            try
            {
                ctx.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(HttpStatusCode.InternalServerError);
            }

            return Content(HttpStatusCode.OK, question);
        }

        protected override void Dispose(bool disposing)
        {
            ctx.Dispose();
            base.Dispose(disposing);
        }
    }
}
