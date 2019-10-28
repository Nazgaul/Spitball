using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Query;
using Cloudents.Query;
using Cloudents.Query.Tutor;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Filters;
using Cloudents.Web.Framework;
using Cloudents.Web.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SbUserManager = Cloudents.Web.Identity.SbUserManager;

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
        /// <param name="term">The term search</param>
        /// <param name="course">The course</param>
        /// <param name="profile"></param>
        /// <param name="page"></param>
        /// <param name="tutorSearch"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("search", Name = "TutorSearch")]
        [ResponseCache(Duration = TimeConst.Hour, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new[] { "*" })]
        public async Task<WebResponseWithFacet<TutorCardDto>> GetAsync(
            string term, string course,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            int page,
            [FromServices] ITutorSearch tutorSearch,

            CancellationToken token)
        {
            term = term ?? string.Empty;
            course = course ?? string.Empty;
            term = $"{term} {course}".Trim();
            //TODO make it better
            if (string.IsNullOrWhiteSpace(term))
            {
                _userManager.TryGetLongUserId(User, out var userId);
                var query = new TutorListQuery(userId, profile.Country, page);
                var result = await _queryBus.QueryAsync(query, token);
                return new WebResponseWithFacet<TutorCardDto>
                {
                    Result = result,
                    NextPageLink = Url.RouteUrl("TutorSearch", new { page = ++page })
                };
            }
            else
            {
                var query = new TutorListTabSearchQuery(term, profile.Country, page);
                var result = await tutorSearch.SearchAsync(query, token);
                return new WebResponseWithFacet<TutorCardDto>
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
        [ResponseCache(Duration = TimeConst.Minute * 5, Location = ResponseCacheLocation.Client, VaryByQueryKeys = new[] { "*" })]
        public async Task<IEnumerable<TutorCardDto>> GetTutorsAsync(
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new TutorListQuery(userId, profile.Country, 0);
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }

        /// <summary>
        /// Return relevant tutors base on user course - on specific course tab - feed
        /// </summary>
        /// <param name="course">The course name</param>
        /// <param name="profile"></param>
        /// <param name="count">Number of tutor result</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = TimeConst.Hour, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })]
        public async Task<IEnumerable<TutorCardDto>> GetTutorsAsync([RequiredFromQuery] string course,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            int? count,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new TutorListByCourseQuery(course, userId, profile.Country, count.GetValueOrDefault(10));
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal;
        }

        [HttpPost("request"), ValidateRecaptcha("6LfyBqwUAAAAALL7JiC0-0W_uWX1OZvBY4QS_OfL"), ValidateEmail]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RequestTutorAsync(RequestTutorRequest model,
            [FromServices] IIpToLocation ipLocation,
            [FromServices] TelemetryClient client,
            [FromHeader(Name = "referer")] Uri referer,
            CancellationToken token)
        {

            if (_userManager.TryGetLongUserId(User, out var userId))
            {
                var query = new UserEmailInfoQuery(userId);
                var userInfo = await _queryBus.QueryAsync(query, token);
                model.Phone = userInfo.PhoneNumber;
                model.Name = userInfo.Name;
                model.Email = userInfo.Email;
            }
            else
            {
                if (model.Email == null)
                {
                    ModelState.AddModelError("error", _stringLocalizer["Need to have email"]);

                    client.TrackTrace("Need to have email 1");
                    return BadRequest(ModelState);
                }
                if (model.Phone == null)
                {
                    ModelState.AddModelError("error", _stringLocalizer["Need to have phone"]);
                    client.TrackTrace("Need to have phone 2");
                    return BadRequest(ModelState);
                }
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (user.PhoneNumber == null)
                    {

                        var location = await ipLocation.GetAsync(HttpContext.Connection.GetIpAddress(), token);
                        var result = await _userManager.SetPhoneNumberAndCountryAsync(user, model.Phone, location?.CallingCode, token);
                        if (result != IdentityResult.Success)
                        {
                            if (string.Equals(result.Errors.First().Code, "Duplicate",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                client.TrackTrace("Invalid Phone number");
                                ModelState.AddModelError("error", _stringLocalizer["Phone number Already in use"]);
                                return BadRequest(ModelState);
                            }
                            client.TrackTrace("Invalid Phone number");
                            ModelState.AddModelError("error", _stringLocalizer["Invalid Phone number"]);
                            return BadRequest(ModelState);
                        }
                    }
                    userId = user.Id;

                }
                else
                {
                    user = new User(model.Email, CultureInfo.CurrentCulture)
                    {
                        Name = model.Name,
                    };
                    var createUserCommand = new CreateUserCommand(user, model.Course);
                    await _commandBus.DispatchAsync(createUserCommand, token);

                    var location = await ipLocation.GetAsync(HttpContext.Connection.GetIpAddress(), token);
                    var result = await _userManager.SetPhoneNumberAndCountryAsync(user, model.Phone, location?.CallingCode, token);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError("error", _stringLocalizer["Invalid Phone number"]);

                        client.TrackTrace("Invalid Phone number 2");
                        return BadRequest(ModelState);
                    }
                    userId = user.Id;
                }
            }

            try
            {
                var utmSource = referer.ParseQueryString()["utm_source"];
                var command = new RequestTutorCommand(model.Course,
                    _stringLocalizer["RequestTutorChatMessage", model.Course, model.Text ?? string.Empty],
                    userId,

                    referer.AbsoluteUri,
                    model.Text, model.TutorId, utmSource, model.MoreTutors);
                await _commandBus.DispatchAsync(command, token);
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("error", _stringLocalizer["You cannot request tutor to yourself"]);
                return BadRequest(ModelState);
            }
            catch (SqlConstraintViolationException)
            {
                client.TrackTrace("Invalid Course");
                ModelState.AddModelError("error", _stringLocalizer["Invalid Course"]);
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpGet("reviews")]
        public async Task<IEnumerable<AboutTutorDto>> GetReviewsAsync(CancellationToken token)
        {
            var query = new AboutTutorQuery();
            var retValTask = await _queryBus.QueryAsync(query, token);
            return retValTask;
        }


        [HttpPost("calendar/Access"), Authorize]
        public async Task<IActionResult> AccessCalendarAsync([FromBody] GoogleCalendarAuth model,
            [FromServices] ICalendarService calendarService,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var url = Request.GetUri();
            var baseUrl = $"{url.Scheme}://{url.Authority}{Url.Content("~")}";

            await calendarService.SaveTokenAsync(model.Code, userId, baseUrl, token);
            return Ok();
        }

        /// <summary>
        /// Get user calendars from google
        /// </summary>
        /// <param name="calendarService"></param>
        /// <param name="token"></param>
        /// <returns>the names of google calendars</returns>
        [HttpGet("calendar/list"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IEnumerable<CalendarDto>> GetTutorCalendarAsync(
            [FromServices] ICalendarService calendarService,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var res = await calendarService.GetUserCalendarsAsync(userId, token);
            return res;
        }

        [HttpPost("calendar/list"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PostTutorCalendarAsync(
            [FromBody] IEnumerable<SetCalendarRequest> model,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command =
                new AddTutorCalendarsCommand(userId, model.Select(s => new AddTutorCalendarsCommand.Calendar(s.Id, s.Name)));
            await _commandBus.DispatchAsync(command, token);
            //var res = await calendarService.GetUserCalendarsAsync(userId, token);
            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns>Return the busy date time</returns>
        [HttpGet("calendar/events"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(555)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetTutorCalendarAsync(
            [FromQuery]CalendarEventRequest model,
            CancellationToken token)
        {
            try
            {
                var query = new CalendarEventsQuery(model.TutorId, model.From.GetValueOrDefault(DateTime.UtcNow), model.To.GetValueOrDefault(DateTime.UtcNow.AddMonths(1)));
                var res = await _queryBus.QueryAsync(query, token);
                return res.BusySlot.ToList();
            }
            catch (NotFoundException)
            {
                return StatusCode(555, new { massege = "permission denied" });
            }
        }


        [HttpPost("calendar/events"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> SetTutorCalendarAsync(
            [FromBody]CalendarEventRequest model,
            CancellationToken token)
        {
            try
            {
                var userId = _userManager.GetLongUserId(User);

                Debug.Assert(model.From != null, "model.From != null");
                Debug.Assert(model.To != null, "model.To != null");

                var command = new AddTutorCalendarEventCommand(userId, model.TutorId, model.From.Value, model.To.Value);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("x", "slot taken");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("calendar/hours"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> SetTutorHoursAsync(
            [FromBody] TutorHoursRequest model,
            CancellationToken token)
        {

            var userId = _userManager.GetLongUserId(User);
            var command = new SetTutorHoursCommand(userId, model.TutorDailyHours.Select(s => new SetTutorHoursCommand.TutorDailyHours(s.Day, s.From, s.To)));
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }
    }
}
