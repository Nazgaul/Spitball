using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Cloudents.MobileApp2.Controllers
{
    [AuthorizeLevel(AuthorizationLevel.User)]
    public class SearchController : ApiController
    {
        public IBoxReadSearchProvider BoxSearchService { get; set; }
        public IItemReadSearchProvider ItemSearchService { get; set; }

        public IUniversityReadSearchProvider UniversitySearch { get; set; }
        public ApiServices Services { get; set; }

        // GET api/Search
        [HttpGet]
        [Route("api/search/boxes")]
        public async Task<HttpResponseMessage> Boxes(string term, int page)
        {
            long? universityId = User.GetUniversityDataId();
            //var userDetail = FormsAuthenticationService.GetUserData();


            if (!universityId.HasValue)
                return Request.CreateBadRequestResponse("need university");


            var query = new SearchQuery(term, User.GetCloudentsUserId(), universityId.Value, page, 20);
            var retVal = await BoxSearchService.SearchBox(query, default(CancellationToken)) ?? new List<SearchBoxes>();

            return Request.CreateResponse(retVal.Select(s => new
             {
                 s.Id,
                 s.Name,
                 s.Professor,                 
                 s.CourseCode,
                 s.Url
             }));
        }

        [HttpGet]
        [Route("api/search/items")]
        public async Task<HttpResponseMessage> Items(string term, int page)
        {
            long? universityId = User.GetUniversityDataId();
            if (!universityId.HasValue)
                return Request.CreateBadRequestResponse("need university");

            var query = new SearchQuery(term, User.GetCloudentsUserId(), universityId.Value, page, 20);
            var retVal = await ItemSearchService.SearchItem(query, default(CancellationToken)) ?? new List<SearchItems>();
            return Request.CreateResponse(retVal.Select(s => new
            {
                s.Name,
                s.Id,
                s.Url
                
            }));
        }

        [HttpGet]
        [Route("api/search/university")]
        [AuthorizeLevel(AuthorizationLevel.Application)]
        public async Task<HttpResponseMessage> University(string term, int page)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Request.CreateBadRequestResponse();
            }
            //try
            //{
            var retVal = await UniversitySearch.SearchUniversity(new UniversitySearchQuery(term, 20, page));
            return Request.CreateResponse(retVal);
            //}
            //catch (Exception ex)
            //{
            //    TraceLog.WriteError("SeachUniversity term:  " + term, ex);
            //    return JsonError();
            //}
        }
    }
}
