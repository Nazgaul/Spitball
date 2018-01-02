using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Web.Http;
using Zbang.Cloudents.Jared.Extensions;
using Zbang.Cloudents.Jared.Filters;
using Zbang.Cloudents.Jared.Models;

namespace Zbang.Cloudents.Jared.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Tutor api controller
    /// </summary>
    [MobileAppController, ApiVersion("2017-12-19")]
    [ControllerName(nameof(TutorController))]
    public class Tutor2Controller : ApiController
    {
        private readonly ITutorSearch _tutorSearch;
        private readonly IIpToLocation _ipToLocation;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tutorSearch"></param>
        /// <param name="ipToLocation"></param>
        public Tutor2Controller(ITutorSearch tutorSearch, IIpToLocation ipToLocation)
        {
            _tutorSearch = tutorSearch;
            _ipToLocation = ipToLocation;
        }

        

        /// <summary>
        /// Get Tutors with next page token add to query string api-version = 2017-12-19
        /// </summary>
        /// <param name="model">The model to parse</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet, Route("api/tutor", Name = "TutorSearch")]
        public async Task<HttpResponseMessage> TutorV2Async([FromUri]TutorRequest model, CancellationToken token)
        {
            if (model.Location == null)
            {
                var location = Request.GetClientIp();
                var locationResult = await _ipToLocation.GetAsync(IPAddress.Parse(location), token).ConfigureAwait(false);
                model.Location = locationResult.ConvertToPoint();
            }
            var result = await _tutorSearch.SearchAsync(model.Term,
                model.Filter,
                model.Sort.GetValueOrDefault(TutorRequestSort.Price),
                model.Location,
                model.Page.GetValueOrDefault(), token).ConfigureAwait(false);
            var nextPageLink = Url.NextPageLink("TutorSearch", new
            {
                api_version = "2017-12-19"
            }, model);
            return Request.CreateResponse(new
            {
                result,
                nextPageLink
            });
        }
    }
}
