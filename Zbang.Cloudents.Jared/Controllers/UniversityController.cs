using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Extensions;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Cloudents.Jared.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// University api controller
    /// </summary>
    [MobileAppController]
    public class UniversityController : ApiController
    {
        private readonly IZboxWriteService _zboxWriteService;
        private readonly IUniversitySearch _universityProvider;

        /// <inheritdoc />
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="zboxWriteService"></param>
        /// <param name="universityProvider"></param>
        public UniversityController(IZboxWriteService zboxWriteService, IUniversitySearch universityProvider)
        {
            _zboxWriteService = zboxWriteService;
            _universityProvider = universityProvider;
        }

        [HttpPost, Authorize, Obsolete]
        public async Task<HttpResponseMessage> Post(CreateUniversityRequest model)
        {
            if (model == null)
            {
                return Request.CreateBadRequestResponse();
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse();
            }
            var command = new CreateUniversityCommand(model.Name, model.Country, User.GetUserId());
            await _zboxWriteService.CreateUniversityAsync(command).ConfigureAwait(false);

            return Request.CreateResponse(command.Id);
        }


        /// <summary>
        /// Get list of universities
        /// </summary>
        /// <param name="model">object of query string</param>
        /// <param name="token"></param>
        /// <returns>list of universities</returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Get([FromUri] UniversityRequest model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateBadRequestResponse(ModelState.GetError());
            }
            var result = await _universityProvider.SearchAsync(model.Term, model.Location, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
