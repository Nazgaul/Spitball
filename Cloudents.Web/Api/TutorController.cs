﻿using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.DTOs;
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
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SbUserManager = Cloudents.Web.Identity.SbUserManager;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.Extension;

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
        private readonly IUrlBuilder _urlBuilder;


        public TutorController(IQueryBus queryBus, SbUserManager userManager,
            ICommandBus commandBus, IStringLocalizer<TutorController> stringLocalizer, IUrlBuilder urlBuilder)
        {
            _queryBus = queryBus;
            _userManager = userManager;
            _commandBus = commandBus;
            _stringLocalizer = stringLocalizer;
            _urlBuilder = urlBuilder;
        }


        /// <summary>
        /// Used for tutor tab
        /// </summary>
        /// <param name="term">The term search</param>
        /// <param name="course">The course</param>
        /// <param name="profile"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="tutorSearch"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("search", Name = "TutorSearch")]
        [ResponseCache(Duration = TimeConst.Hour, Location = ResponseCacheLocation.Client,
            VaryByQueryKeys = new[] {"*"})]
        public async Task<WebResponseWithFacet<TutorCardDto>> GetAsync(
            string term, string course,
            [ProfileModelBinder(ProfileServiceQuery.Country)]
            UserProfile profile,
            int page,
            [FromServices] ITutorSearch tutorSearch,
            CancellationToken token, int pageSize = 20)
        {
            term ??= string.Empty;
            course ??= string.Empty;
            term = $"{term} {course}".Trim();
            //TODO make it better
            if (string.IsNullOrWhiteSpace(term))
            {
                _userManager.TryGetLongUserId(User, out var userId);
                var query = new TutorListQuery(userId, profile.CountryRegion, page, pageSize);
                var result = await _queryBus.QueryAsync(query, token);
                result.Result = result.Result.Select(s =>
                {
                    s.Image = _urlBuilder.BuildUserImageEndpoint(s.UserId, s.Image);
                    return s;
                });

                return new WebResponseWithFacet<TutorCardDto>
                {
                    Result = result.Result,
                    Count = result.Count,
                    NextPageLink = Url.RouteUrl("TutorSearch", new {page = ++page})
                };
            }
            else
            {
                var query = new TutorListTabSearchQuery(term, profile.CountryRegion, page, pageSize);
                var result = await tutorSearch.SearchAsync(query, token);
                return new WebResponseWithFacet<TutorCardDto>
                {
                    Result = result.Result,
                    NextPageLink = Url.RouteUrl("TutorSearch", new {page = ++page, term}),
                    Count = result.Count
                };
            }
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
        [ResponseCache(Duration = TimeConst.Hour, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] {"*"})]
        public async Task<IEnumerable<TutorCardDto>> GetTutorsAsync([RequiredFromQuery] string course,
            [ProfileModelBinder(ProfileServiceQuery.Country)]
            UserProfile profile,
            int? count,
            CancellationToken token)
        {
            _userManager.TryGetLongUserId(User, out var userId);
            var query = new TutorListByCourseQuery(course, userId, profile.CountryRegion, count.GetValueOrDefault(10));
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal.Select(item =>
            {
                item.Image = _urlBuilder.BuildUserImageEndpoint(item.UserId, item.Image);
                return item;
            });

        }

        [HttpPost("request"), ValidateRecaptcha("6LfyBqwUAAAAALL7JiC0-0W_uWX1OZvBY4QS_OfL"), ValidateEmail]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary),
            StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [Authorize]
        public async Task<IActionResult> RequestTutorAsync(RequestTutorRequest model,
            [FromServices] TelemetryClient client,
            [FromHeader(Name = "referer")] Uri referer,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            //if (!_userManager.TryGetLongUserId(User, out var userId))
            //{
            //    if (model.Email == null)
            //    {
            //        ModelState.AddModelError("error", _stringLocalizer["Need to have email"]);

            //        client.TrackTrace("Need to have email 1");
            //        return BadRequest(ModelState);
            //    }

            //    if (model.Phone == null)
            //    {
            //        ModelState.AddModelError("error", _stringLocalizer["Need to have phone"]);
            //        client.TrackTrace("Need to have phone 2");
            //        return BadRequest(ModelState);
            //    }


            //    var query = new CountryByIpQuery(HttpContext.GetIpAddress().ToString());
            //    var location = await _queryBus.QueryAsync(query, token);

            //    var user = await _userManager.FindByEmailAsync(model.Email);
            //    if (user != null)
            //    {
            //        if (user.PhoneNumber == null)
            //        {

            //            var result =
            //                await _userManager.SetPhoneNumberAndCountryAsync(user, model.Phone, location?.CallingCode,
            //                    token);
            //            if (result != IdentityResult.Success)
            //            {
            //                if (string.Equals(result.Errors.First().Code, "Duplicate",
            //                    StringComparison.OrdinalIgnoreCase))
            //                {
            //                    client.TrackTrace("Invalid Phone number");
            //                    ModelState.AddModelError("error", _stringLocalizer["Phone number Already in use"]);
            //                    return BadRequest(ModelState);
            //                }

            //                client.TrackTrace("Invalid Phone number");
            //                ModelState.AddModelError("error", _stringLocalizer["Invalid Phone number"]);
            //                return BadRequest(ModelState);
            //            }
            //        }

            //        userId = user.Id;
            //    }
            //    else
            //    {
            //        user = await _userManager.FindByPhoneAsync(model.Phone, location?.CallingCode);
            //        if (user != null)
            //        {
            //            userId = user.Id;
            //        }
            //        else
            //        {
            //            var country = await countryService.GetUserCountryAsync(token);

            //            user = new User(model.Email, model.Name, null, CultureInfo.CurrentCulture, country);

            //            var createUserCommand = new CreateUserCommand(user, model.Course);
            //            await _commandBus.DispatchAsync(createUserCommand, token);

            //            var result =
            //                await _userManager.SetPhoneNumberAndCountryAsync(user, model.Phone, location?.CallingCode,
            //                    token);
            //            if (result != IdentityResult.Success)
            //            {
            //                ModelState.AddModelError("error", _stringLocalizer["Invalid Phone number"]);

            //                client.TrackTrace("Invalid Phone number 2");
            //                return BadRequest(ModelState);
            //            }

            //            userId = user.Id;
            //        }
            //    }
            //}

            try
            {
                var queryString = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(referer.Query);
                queryString.TryGetValue("utm_source", out var utmSource);
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

            if (model.TutorId.HasValue)
            {
                var query = new GetPhoneNumberQuery(model.TutorId.Value);
                var val = await _queryBus.QueryAsync(query, token);
                return Ok(new
                {
                    PhoneNumber = val
                });
            }

            return Ok();
        }

        [HttpGet("reviews")]
        public async Task<IEnumerable<AboutTutorDto>> GetReviewsAsync(CancellationToken token)
        {
            var query = new AboutTutorQuery();
            var retVal = await _queryBus.QueryAsync(query, token);
            return retVal.Select(item =>
            {
                item.Image = _urlBuilder.BuildUserImageEndpoint(item.UserId, item.Image);
                return item;
            });

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
        /// <param name="token"></param>
        /// <returns>the names of google calendars</returns>
        [HttpGet("calendar/list"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(555)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<CalendarDto>>> GetTutorCalendarAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            try
            {
                var query = new CalendarListQuery(userId);
                return (await _queryBus.QueryAsync(query, token)).ToList();
            }
            catch (NotFoundException)
            {
                return StatusCode(555, new {massege = "permission denied"});
            }
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
                new AddTutorCalendarsCommand(userId,
                    model.Select(s => new AddTutorCalendarsCommand.Calendar(s.Id, s.Name)));
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
            [FromQuery] CalendarEventRequest model,
            CancellationToken token)
        {
            try
            {
                var query = new CalendarEventsQuery(model.TutorId,
                    model.From.GetValueOrDefault(DateTime.UtcNow),
                    model.To.GetValueOrDefault(DateTime.UtcNow.AddMonths(1)));
                var res = await _queryBus.QueryAsync(query, token);
                return res.BusySlot.ToList();
            }
            catch (NotFoundException)
            {
                return StatusCode(555, new {massege = "permission denied"});
            }
        }


        [HttpPost("calendar/events"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary),
            StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> SetTutorCalendarAsync(
            [FromBody] CalendarSetEvent model,
            CancellationToken token)
        {
            try
            {
                var userId = _userManager.GetLongUserId(User);

                Debug.Assert(model.From != null, "model.From != null");

                var command = new AddTutorCalendarEventCommand(userId, model.TutorId, model.From.Value,
                    model.From.Value.AddHours(1));
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
            var command = new UpdateTutorHoursCommand(userId,
                model.TutorDailyHours.Select(s => new TutorAvailabilitySlot(s.Day, s.From, s.To)));
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("{tutorId:long}/subscribe"),Authorize]
        public async Task<IActionResult> SubscribeToTutorAsync(
            long tutorId,
            [FromHeader(Name = "referer")] string referer,
            [FromServices] IStripeService stripeService,
            CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User);
            var email = await _userManager.GetEmailAsync(user);

            var uriBuilder = new UriBuilder(referer)
            {
                Query = string.Empty
            };
            var url = new UriBuilder(Url.RouteUrl("stripe-subscribe", new
            {
                redirectUrl = uriBuilder.ToString()
            }, "https"));
            var successCallback = url.AddQuery(("sessionId", "{CHECKOUT_SESSION_ID}"), false).ToString();

            var result = await stripeService.SubscribeToTutorAsync(tutorId, email, successCallback, referer, token);
            return Ok(new
            {
                sessionId = result
            });
        }
    }
}
