using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Framework;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
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
        private readonly UserManager<User> _userManager;
        private readonly ICommandBus _commandBus;
        private readonly IStringLocalizer<TutorController> _stringLocalizer;


        public TutorController(IQueryBus queryBus, UserManager<User> userManager,
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
            [FromServices] ISmsSender client,
            [FromHeader(Name = "referer")] Uri referer,
            CancellationToken token)
        {

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
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (user.PhoneNumber == null)
                    {
                        var phoneNumber = await client.ValidateNumberAsync(model.ToString(), token);
                        if (phoneNumber.phoneNumber != null)
                        {
                            user.Country = phoneNumber.country;
                            await _userManager.SetPhoneNumberAsync(user, model.Phone);
                            userId = user.Id;
                        }
                    }
                    else
                    {
                        userId = user.Id;
                    }
                }
                else
                {

                }
                //TODO : need to register user
            }

            // if (userId > 0)
            // {
            var command = new RequestTutorCommand(model.Course,
                _stringLocalizer["RequestTutorChatMessage"],
                userId,
                model.University,
                referer.AbsoluteUri,
                model.Name,
                model.Phone,
                model.Text,model.Email);
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
