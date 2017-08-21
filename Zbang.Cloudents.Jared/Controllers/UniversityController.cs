using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
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
        private readonly IZboxWriteService m_ZboxWriteService;

        public UniversityController(IZboxWriteService zboxWriteService)
        {
            m_ZboxWriteService = zboxWriteService;
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
            await m_ZboxWriteService.CreateUniversityAsync(command).ConfigureAwait(false);

            return Request.CreateResponse(command.Id);

        }
    }
}
