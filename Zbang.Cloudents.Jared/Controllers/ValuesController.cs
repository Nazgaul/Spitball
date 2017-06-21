using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using WebApi.OutputCache.V2;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.ReadServices;
using System.Net;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Infrastructure.Mail;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController]
    public class ValuesController : ApiController
    {
        private readonly IZboxReadService m_ZboxReadService;
        private readonly IMailComponent m_MailComponent;

        public ValuesController(IZboxReadService zboxReadService, IMailComponent mailComponent)
        {
            m_ZboxReadService = zboxReadService;
            m_MailComponent = mailComponent;
        }

        // GET api/values
        [CacheOutput(ClientTimeSpan = TimeConst.Day, ServerTimeSpan = TimeConst.Day)]
        public async Task<HttpResponseMessage> Get(CancellationToken token)
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
            var result = await m_ZboxReadService.GetJaredStartupValuesAsync(token).ConfigureAwait(false);
            //}
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
            //result.Terms = documents;
            return Request.CreateResponse(new
            {
                text = result,
                terms = documents
            });
        }

        [HttpPost]
        [Route("api/feedback")]
        public async Task<HttpResponseMessage> FeedbackAsync(MailRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            await m_MailComponent.GenerateSystemEmailAsync("Jared feedback", model.Body, "support@cloudents.com").ConfigureAwait(false);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
