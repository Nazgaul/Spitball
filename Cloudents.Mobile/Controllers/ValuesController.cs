using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    [Obsolete]
    public class ValuesController : ApiController
    {
        private readonly IZboxReadService m_ZboxReadService;

        public ValuesController(IZboxReadService zboxReadService)
        {
            m_ZboxReadService = zboxReadService;
        }

        // GET api/values
        public HttpResponseMessage Get()
        {
            //JaredDto result=null;
            //bool isAuthorized = User.Identity.IsAuthenticated;
            //if (User.Identity.IsAuthenticated)
            //{
            //    result = await m_ZboxReadService
            //        .GetJaredStartupValuesAsync(token, new QueryBaseUserId(User.GetUserId()))
            //        .ConfigureAwait(false);
            //}
            //else
            //{
            //var result = await m_ZboxReadService.GetJaredStartupValuesAsync(token).ConfigureAwait(false);
            //}
            var documents = new Dictionary<string, IEnumerable<string>>
            {
                ["exams"] = new[] { "exam", "test", "midterm", "final", "tests", "midterms", "finals" },
                [
                    "quizzes"] =
                    new[] { "quiz", "flashcard", "set", "quizlet", "flashcards", "flash cards", "quizlets", "sets" },
                ["study guides"] = new[] { "study guide", "review", "guide", "reviews", "guides" },
                ["homework"] = new[] { "hw", "assignments", "assignment" },
                ["lectures"] = new[] { "lecture" },
                ["class notes"] = new[] { "class note", "note", "notes" }
            };

            //result.Terms = documents;
            return Request.CreateResponse(new
            {
                verticals = new[] { 1, 0, 2, 3, 4, 5, 6, 7, 8 },
                // text = result,
                terms = documents
            });
        }
    }
}
