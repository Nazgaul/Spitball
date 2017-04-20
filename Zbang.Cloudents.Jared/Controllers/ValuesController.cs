

using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using WebApi.OutputCache.V2;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.Infrastructure.Enums;
using System;

namespace Zbang.Cloudents.Jared.Controllers
{
    // Use the MobileAppController attribute for each ApiController you want to use  
    // from your mobile clients 
    [MobileAppController]
    //[Authorize]
    public class ValuesController : ApiController
    {
        private readonly IZboxReadService m_ZboxReadService;

        public ValuesController(IZboxReadService zboxReadService)
        {
            m_ZboxReadService = zboxReadService;
        }

        // GET api/values
        [CacheOutput(ClientTimeSpan = TimeConst.Day, ServerTimeSpan = TimeConst.Day)]
        public async Task<HttpResponseMessage> Get(CancellationToken token)
        {
           var result = await m_ZboxReadService.GetJaredStartupValuesAsync(token).ConfigureAwait(false);
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
            result.Terms = documents;


            return Request.CreateResponse(result);
        }

        //private async Task<Dictionary<CategoryTextType,string>> getCategoriesText()
        //{
        //    var dict = new Dictionary<CategoryTextType, string>();
        //    var vals = await m_ZboxReadService.GetJaredText();
            
        //    foreach (CategoryTextType item in Enum.GetValues(typeof(CategoryTextType)))
        //    {
        //        dict.Add(item, "hello"); 
        //    }
        //    return dict;
        //}

        //// POST api/values
        //public string Post()
        //{
        //    return "Hello World!";
        //}
    }
}
