using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;

namespace Zbang.Cloudents.Jared.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    public class ValuesController : ApiController
    {
        // GET api/values
        public HttpResponseMessage Get()
        {
            var documents = new Dictionary<string, IEnumerable<string>>();
            documents.Add("exams", new[] {"exam", "test", "midterm", "final", "tests", "midterms", "finals"});
            documents.Add("quizzes",
                new[] {"quiz", "flashcard", "set", "quizlet", "flashcards", "flash cards", "quizlets", "sets"});
            documents.Add("study guides", new[] {"study guide", "review", "guide", "reviews", "guides"});
            documents.Add("homework", new[] {"hw", "assignments", "assignment"});
            documents.Add("lectures", new[] { "lecture" });
            documents.Add("class notes", new[] {"class note", "note", "notes"});

            return Request.CreateResponse(documents);
        }

        // POST api/values
        public string Post()
        {
            return "Hello World!";
        }
    }
}
