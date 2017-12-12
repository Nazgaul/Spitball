using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Microsoft.Azure.Mobile.Server.Config;
using Zbang.Cloudents.Jared.Models;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Cloudents.Jared.Controllers
{
    [MobileAppController,Authorize]
    public class UniversityController : ApiController
    {
        private readonly IZboxWriteService _zboxWriteService;
        private readonly IUniversitySearch _universityProvider;

        public UniversityController(IZboxWriteService zboxWriteService, IUniversitySearch universityProvider)
        {
            _zboxWriteService = zboxWriteService;
            _universityProvider = universityProvider;
        }

        [HttpPost]
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


        [HttpGet]
        public async Task<HttpResponseMessage> Get(string term, GeoPoint location, CancellationToken token)
        {
            var result = await _universityProvider.SearchAsync(term, location, token).ConfigureAwait(false);
            return Request.CreateResponse(result);
        }
    }
}
