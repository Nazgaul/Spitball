using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Enum;
using Cloudents.Query;
using Cloudents.Query.Questions;
using Cloudents.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Schema.NET;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class QuestionController : Controller
    {
        private readonly IQueryBus _queryBus;
        private readonly IStringLocalizer<QuestionController> _localizer;

        public QuestionController(IQueryBus queryBus, IStringLocalizer<QuestionController> localizer)
        {
            _queryBus = queryBus;
            _localizer = localizer;
        }

        [Route("question/{id:long}", Name = SeoTypeString.Question), SignInWithToken]
        // GET: /<controller>/
        public async Task<IActionResult> IndexAsync(long id, CancellationToken token)
        {
            var query = new QuestionDataByIdQuery(id);
            var retVal = await _queryBus.QueryAsync(query, token);
            if (retVal == null)
            {
                return NotFound();
            }
            ViewBag.title = _localizer["Title", retVal.Course];
            ViewBag.metaDescription = _localizer["Description", retVal.Course];

            if (!retVal.Answers.Any()) return View("Index");
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
                    SuggestedAnswer = new Values<IAnswer, IItemList>(retVal.Answers.Select((s, i) =>
                        new Answer
                        {
                            //Text = s.Text,
                            DateCreated = s.Create,
                            Author = new Person
                            {
                                Name = s.User.Name
                            },
                            Url = new Uri(Url.RouteUrl(SeoTypeString.Question, new { id }, "https", null, $"answer-{i}"))
                        })),

                }
            };
            ViewBag.jsonLd = jsonLd;

            return View("Index");
        }
    }
}
