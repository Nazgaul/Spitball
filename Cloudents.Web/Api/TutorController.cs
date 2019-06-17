using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message;
using Cloudents.Core.Message.Email;
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
        private readonly UserManager<RegularUser> _userManager;
        private readonly IMondayProvider _mondayProvider;
        private readonly ICommandBus _commandBus;
        private readonly IStringLocalizer<TutorController> _stringLocalizer;


        public TutorController(IQueryBus queryBus, UserManager<RegularUser> userManager,
            IMondayProvider mondayProvider, ICommandBus commandBus, IStringLocalizer<TutorController> stringLocalizer)
        {
            _queryBus = queryBus;
            _userManager = userManager;
            _mondayProvider = mondayProvider;
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
            [FromServices]  IQueueProvider queueProvider,
            [FromServices] IHostingEnvironment configuration,
            [FromServices] ISmsSender _client,
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
                        var phoneNumber = await _client.ValidateNumberAsync(model.ToString(), token);
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

                //TODO : need to register user
            }

            if (userId > 0)
            {
                var command = new SendTutorRequestChatTextMessageCommand(model.Course,
                    _stringLocalizer["RequestTutorChatMessage"],
                    userId);
                await _commandBus.DispatchAsync(command, token);
            }


            var email = new RequestTutorEmail()
            {
                Email = model.Email,
                IsProduction = configuration.IsProduction()
            };
            foreach (var propertyInfo in model.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(model);
                if (value != null)
                {
                    email.Dictionary.Add(propertyInfo.Name, value.ToString());
                }
            }
            email.Dictionary.Add("Referer", referer.AbsoluteUri);

            var utmSource = referer.ParseQueryString()["utm_source"];
            var task1 = queueProvider.InsertMessageAsync(email, token);
            var task2 = _mondayProvider.CreateRecordAsync(new MondayMessage(model.Course,
                configuration.IsProduction(),
                model.Name,
                model.Phone,
                model.Text,
                model.University,
                utmSource
                ), token);


            await Task.WhenAll(task1, task2);
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
