using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Identity;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Framework;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    /// <inheritdoc />
    /// <summary>
    /// Tutor api controller
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]"), ApiController]
    public class TutorController : ControllerBase
    {
        private readonly IQueryBus _queryBus;
        private readonly SbUserManager _userManager;
        private readonly ICommandBus _commandBus;
        private readonly IStringLocalizer<TutorController> _stringLocalizer;


        public TutorController(IQueryBus queryBus, SbUserManager userManager,
             ICommandBus commandBus, IStringLocalizer<TutorController> stringLocalizer)
        {
            _queryBus = queryBus;
            _userManager = userManager;
            _commandBus = commandBus;
            _stringLocalizer = stringLocalizer;
        }


        /// <summary>
        /// Used for tutor tab
        /// </summary>
        /// <param name="term"></param>
        /// <param name="profile"></param>
        /// <param name="page"></param>
        /// <param name="tutorSearch"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("search", Name = "TutorSearch")]
        [ResponseCache(Duration = TimeConst.Hour, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new[] { "*" })]
        public async Task<WebResponseWithFacet<TutorListDto>> GetAsync(
            string term,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            int page,
            [FromServices] ITutorSearch tutorSearch,

            CancellationToken token)
        {
            //TODO make it better
            if (string.IsNullOrEmpty(term))
            {
                var query = new TutorListTabQuery(profile.Country, page: page);
                var result = await _queryBus.QueryAsync(query, token);
                return new WebResponseWithFacet<TutorListDto>
                {
                    Result = result,
                    NextPageLink = Url.RouteUrl("TutorSearch", new { page = ++page })
                };
            }
            else
            {
                var query = new TutorListTabSearchQuery(term, profile.Country, page);
                var result = await tutorSearch.SearchAsync(query, token);
                return new WebResponseWithFacet<TutorListDto>
                {
                    Result = result,
                    NextPageLink = Url.RouteUrl("TutorSearch", new { page = ++page, term })
                };
            }
        }






        /// <summary>
        /// Return relevant tutors base on user courses -on all courses tab - feed
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<TutorListDto>> GetTutorsAsync(
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new TutorListQuery(userId, profile.Country);
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }

        /// <summary>
        /// Return relevant tutors base on user course - on specific course tab - feed
        /// </summary>
        /// <param name="courseName">The course name</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<TutorListDto>> GetTutorsAsync([RequiredFromQuery] string courseName,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new TutorListByCourseQuery(courseName, userId);
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }
        [HttpPost("request")]
        public async Task<IActionResult> RequestTutorAsync(RequestTutorRequest model,
            [FromServices] IIpToLocation ipLocation,
            [FromHeader(Name = "referer")] Uri referer,
            CancellationToken token)
        {
            //var phoneNumber = await client.ValidateNumberAsync(model.ToString(), token);

            //RequestTutorEmail
            if (_userManager.TryGetLongUserId(User, out var userId))
            {
                var query = new UserEmailInfoQuery(userId);
                var userInfo = await _queryBus.QueryAsync(query, token);
                model.Phone = userInfo.PhoneNumber;
                model.Name = userInfo.Name;
                model.Email = userInfo.Email;
                model.University = userInfo.University;
            }
            else
            {
                if (model.Email == null)
                {
                    ModelState.AddModelError(nameof(model.Email), "Need to have email");
                    return BadRequest(ModelState);
                }
                if (model.Phone == null)
                {
                    ModelState.AddModelError(nameof(model.Phone), "Need to have phone");
                    return BadRequest(ModelState);
                }
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (user.PhoneNumber == null)
                    {
                        var location = await ipLocation.GetAsync(HttpContext.Connection.GetIpAddress(), token);
                        await _userManager.SetPhoneNumberAndCountryAsync(user, model.Phone, location?.CallingCode, token);
                    }
                    userId = user.Id;

                }
                else
                {

                    user = new User(model.Email, CultureInfo.CurrentCulture)
                    {
                        Name = model.Name,
                    };
                    var createUserCommand = new CreateUserCommand(user, model.University,model.Course);
                    await _commandBus.DispatchAsync(createUserCommand, token);

                    var location = await ipLocation.GetAsync(HttpContext.Connection.GetIpAddress(), token);
                    await _userManager.SetPhoneNumberAndCountryAsync(user, model.Phone, location?.CallingCode, token);
                    userId = user.Id;
                }
            }


            var utmSource = referer.ParseQueryString()["utm_source"];
            var command = new RequestTutorCommand(model.Course,
                _stringLocalizer["RequestTutorChatMessage", model.Course],
                userId,

                referer.AbsoluteUri,
                model.Text, model.TutorId, utmSource);
            await _commandBus.DispatchAsync(command, token);

            return Ok();
        }

        [HttpGet("reviews")]
        public async Task<IEnumerable<AboutTutorDto>> GetReviewsAsync(CancellationToken token)
        {
            var query = new AboutTutorQuery();
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }
    }
}
