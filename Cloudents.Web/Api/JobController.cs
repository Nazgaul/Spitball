using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// The controller of job api
    /// </summary>
    [Route("api/[controller]", Name = "Job"), ApiController]
    public class JobController : ControllerBase
    {
        //private readonly IJobSearch _jobSearch;
        private readonly IStringLocalizer<QuestionController> _questionLocalizer;

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="questionLocalizer"></param>
        public JobController( IStringLocalizer<QuestionController> questionLocalizer)
        {
            _questionLocalizer = questionLocalizer;
        }

        /// <summary>
        /// Query to get jobs vertical
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]

        public async Task<WebResponseWithFacet<JobDto>> GetAsync(/*[FromQuery]JobRequest model,*/ CancellationToken token)
        {
            //var result = await _jobSearch.SearchAsync(model.Term,
            //    model.Sort.GetValueOrDefault(JobRequestSort.Relevance),
            //    model.Facet, model.Location?.ToLocation(), model.Page.GetValueOrDefault(), token);
            //result.Result = result.Result?.ToList();

            //var retVal = new WebResponseWithFacet<JobDto>()
            //{
            //    Result = result.Result,
            //    Sort = EnumExtension.GetValues<JobRequestSort>().Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())),

            //};
            //if (result.Result?.Any() == true)
            //{
            //    retVal.NextPageLink = Url.NextPageLink("Job", model);
            //}

            //if (result.Facet != null)
            //{
            //    retVal.Filters = new IFilters[]
            //    {
            //        new Filters<string>(nameof(JobRequest.Facet), _questionLocalizer["SubjectTypeTitle"],
            //            result.Facet.Select(s => new KeyValuePair<string, string>(s, s)))
            //    };
            //}

            return new WebResponseWithFacet<JobDto>
            {
                Result = new JobDto[0],
                Sort = EnumExtension.GetValues<JobRequestSort>().Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())),
                Filters = new IFilters[]
                {
                    //new Filters<string>(nameof(SearchRequest.Source),_localizer["Sources"],result.Facet.Select(s=> new KeyValuePair<string, string>(s,s)))
                },
                NextPageLink = null
            };
        }
    }
}