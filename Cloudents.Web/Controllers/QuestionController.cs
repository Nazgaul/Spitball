using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Schema.NET;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class QuestionController : Controller
    {
        private readonly IQueryBus _queryBus;

        public QuestionController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [Route("question/{id:long}", Name = SeoTypeString.Question), SignInWithToken]
        // GET: /<controller>/
        public async Task<IActionResult> Index(long id, CancellationToken token)
        {
            var query = new QuestionDataByIdQuery(id);
            var retVal = await _queryBus.QueryAsync(query, token);
            if (retVal == null)
            {
                return NotFound();
            }


            if (!retVal.Answers.Any()) return View();
            var jsonLd = new QAPage()
            {
                MainEntity = new Question
                {
                    DateCreated = retVal.Create,
                    Author = new Person
                    {
                        Name = retVal.User.Name,
                        //Image = retVal.User.Image
                    },
                    Name = retVal.Text,
                    Text = retVal.Text,
                    AnswerCount = retVal.Answers.Count(),
                    SuggestedAnswer = new Values<IAnswer, IItemList>(retVal.Answers.Select(s =>
                        new Answer
                        {
                            Text = s.Text,
                            DateCreated = s.Create,
                            Author = new Person
                            {
                                Name = s.User.Name
                            }
                        })),

                }
            };
            ViewBag.jsonLd = jsonLd;

            return View();
        }
    }
}
