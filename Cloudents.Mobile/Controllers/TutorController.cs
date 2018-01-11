using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Mobile.Models;
using Microsoft.Azure.Mobile.Server.Config;

namespace Cloudents.Mobile.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Tutor api controller
    /// </summary>
    [MobileAppController]
    public class TutorController : ApiController
    {
        private readonly ITutorSearch _tutorSearch;
        private readonly IIpToLocation _ipToLocation;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tutorSearch"></param>
        /// <param name="ipToLocation"></param>
        public TutorController(ITutorSearch tutorSearch, IIpToLocation ipToLocation)
        {
            _tutorSearch = tutorSearch;
            _ipToLocation = ipToLocation;
        }

        /// <summary>
        /// Get Tutors
        /// </summary>
        /// <param name="model">The model to parse</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> Get([FromUri]TutorRequest model, CancellationToken token)
        {
            var result = await _tutorSearch.SearchAsync(model.Term,
                model.Filter,
                model.Sort.GetValueOrDefault(TutorRequestSort.Price),
                model.Location,
                model.Page.GetValueOrDefault(), token).ConfigureAwait(false);

            return Request.CreateResponse(
                result
                );
        }

       
    }
}
