using Cloudents.Web.Extensions;
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
using Cloudents.Core.Interfaces;
using Wangkanai.Detection;
using Microsoft.AspNetCore.Identity;
using Cloudents.Core.Entities;
using Cloudents.Command;
using Cloudents.Command.Command;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Tutor api controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]", Name = "Tutor"), ApiController]
    public class TutorController : ControllerBase
    {
        private readonly ITutorSearch _tutorSearch;
        private readonly IStringLocalizer<TutorController> _localizer;
        private readonly IDevice _device;
       

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tutorSearch"></param>
        /// <param name="localizer"></param>
        /// <param name="deviceResolver"></param>
        public TutorController(ITutorSearch tutorSearch, IStringLocalizer<TutorController> localizer
                , IDeviceResolver deviceResolver)
        {
            _tutorSearch = tutorSearch;
            _localizer = localizer;
            _device = deviceResolver.Device;
        }

        /// <summary>
        /// Get Tutors
        /// </summary>
        /// <param name="model">The model to parse</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<WebResponseWithFacet<TutorDto>> GetAsync([FromQuery]TutorRequest model, CancellationToken token)
        {
            var isMobile = _device.Type == DeviceType.Mobile;
            var result = (await _tutorSearch.SearchAsync(model.Term,
                model.Filter,
                model.Sort.GetValueOrDefault(TutorRequestSort.Relevance),
                model.Location?.ToGeoPoint(),
                model.Page.GetValueOrDefault(), isMobile, token)).ToListIgnoreNull();
            string nextPageLink = null;
            if (result.Count > 0)
            {
                nextPageLink = Url.NextPageLink("Tutor", null, model);
            }

            return new WebResponseWithFacet<TutorDto>
            {
                Result = result,
                Sort = EnumExtension.GetValues<TutorRequestSort>().Select(s => new KeyValuePair<string, string>(s.ToString("G"), s.GetEnumLocalization())),
                Filters = new IFilters[]
                {
                    new Filters<string>(nameof(TutorRequest.Filter),_localizer["StatusFilter"],
                        EnumExtension.GetValues<TutorRequestFilter>()
                            .Select(s=> new KeyValuePair<string, string>(s.ToString("G"),s.GetEnumLocalization())))
                },
                NextPageLink = nextPageLink
            };
        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditUserProfileAsync([FromBody]EditToturProfileRequest model,
            [FromServices] ICommandBus _commandBus, [FromServices] UserManager<RegularUser> _userManager
            , CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new EditToturProfileCommand(userId, model.Name, model.LastName, model.Bio, model.Description);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
