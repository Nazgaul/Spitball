using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using WebApi.OutputCache.V2;
using Zbang.Zbox.Infrastructure.Consts;

namespace Zbang.Cloudents.Jared.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    public class ValuesController : ApiController
    {
        // GET api/values
        [CacheOutput(ClientTimeSpan = TimeConst.Day, ServerTimeSpan = TimeConst.Day)]
        public HttpResponseMessage Get()
        {
            var documents = new Dictionary<string, IEnumerable<string>>
            {
                {"exams", new[] {"exam", "test", "midterm", "final", "tests", "midterms", "finals"}},
                {
                    "quizzes",
                    new[] {"quiz", "flashcard", "set", "quizlet", "flashcards", "flash cards", "quizlets", "sets"}
                },
                {"study guides", new[] {"study guide", "review", "guide", "reviews", "guides"}},
                {"homework", new[] {"hw", "assignments", "assignment"}},
                {"lectures", new[] {"lecture"}},
                {"class notes", new[] {"class note", "note", "notes"}}
            };

            return Request.CreateResponse(documents);
        }

        // POST api/values
        public string Post()
        {
            return "Hello World!";
        }
    }
}
