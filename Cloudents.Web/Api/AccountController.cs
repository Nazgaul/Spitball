using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Storage;
using Cloudents.Query;
using Cloudents.Web.Binders;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Cloudents.Query.Users;
using Cloudents.Query.Tutor;
using Cloudents.Query.Questions;
using Cloudents.Core.DTOs.Users;
using Cloudents.Core.DTOs.Tutors;
using Cloudents.Core.DTOs.Questions;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly IUrlBuilder _urlBuilder;

        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager, ICommandBus commandBus, IQueryBus queryBus, IUrlBuilder urlBuilder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _commandBus = commandBus;
            _queryBus = queryBus;
            _urlBuilder = urlBuilder;
        }

        // GET
        [HttpGet]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        [ProducesResponseType(Status200OK)]
        public async Task<ActionResult<UserAccountDto>> GetAsync(
            [FromServices] IQueryBus queryBus,
            [FromServices] ILogger logger,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserAccountQuery(userId);
            var user = await queryBus.QueryAsync(query, token);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                logger.Error($"User is null {userId}");
                return Unauthorized();
            }
            user.Image = _urlBuilder.BuildUserImageEndpoint(userId, user.Image);
            return user;
        }

        [AllowAnonymous, HttpPost("language")]
        public async Task<IActionResult> ChangeLanguageAsync([FromBody]ChangeCultureRequest model, CancellationToken token)
        {
            var culture = model.Culture;

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(culture),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            if (!User.Identity.IsAuthenticated)
            {
                return Ok();
            }

            var userId = _userManager.GetLongUserId(User);
            var command = new UpdateUserCultureCommand(userId, culture.Culture.Name);
            await _commandBus.DispatchAsync(command, token);

            return Ok();
        }

        [HttpGet("referrals")]
        [ResponseCache(Duration = TimeConst.Minute * 30, Location = ResponseCacheLocation.Client)]
        public async Task<UserReferralsDto> GetReferralsAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserReferralsQuery(userId);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpPost("image")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadImageAsync([Required] IFormFile file,
            [FromServices] IUserDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            Uri uri;
            try
            {
                uri = await blobProvider.UploadImageAsync(userId, file.FileName, file.OpenReadStream(), file.ContentType, token);
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("x", "not an image");
                return BadRequest(ModelState);
            }

            if (uri == null)
            {
                ModelState.AddModelError("x", "not an image");
                return BadRequest(ModelState);
            }
            var imageProperties = new ImageProperties(uri, ImageProperties.BlurEffect.None);
            var url = Url.ImageUrl(imageProperties);
            var fileName = uri.AbsolutePath.Split('/').LastOrDefault();
            var command = new UpdateUserImageCommand(userId, url, fileName);
            await _commandBus.DispatchAsync(command, token);
            return Ok(url);
        }


        [HttpPost("cover")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary), Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadCoverImageAsync([Required] IFormFile file,
            [FromServices] IUserDirectoryBlobProvider blobProvider,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            Uri uri;
            try
            {
                uri = await blobProvider.UploadImageAsync(userId, file.FileName, file.OpenReadStream(), file.ContentType, token);
            }
            catch (ArgumentException)
            {
                ModelState.AddModelError("x", "not an image");
                return BadRequest(ModelState);
            }

            if (uri == null)
            {
                ModelState.AddModelError("x", "not an image");
                return BadRequest(ModelState);
            }
            //   var imageProperties = new ImageProperties(uri, ImageProperties.BlurEffect.None);
            //var url = Url.ImageUrl(imageProperties);

            var fileName = uri.AbsolutePath.Split('/').LastOrDefault();
            var command = new UpdateUserCoverImageCommand(userId,  fileName);
            await _commandBus.DispatchAsync(command, token);

            var url = _urlBuilder.BuildUserImageEndpoint(userId, fileName);
            return Ok(url);
        }


        [HttpPost("settings")]
        public async Task<IActionResult> ChangeSettingsAsync(
            [FromBody]UpdateSettingsRequest model,
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new UpdateUserSettingsCommand(userId, model.FirstName, model.LastName,
                model.Description, model.Bio, model.Price);
            await _commandBus.DispatchAsync(command, token);
            var culture = CultureInfo.CurrentCulture.ChangeCultureBaseOnCountry(profile.Country);
            return Ok(new
            {
                newPrice = model.Price?.ToString("C0", culture)
            });
        }

        [HttpPost("BecomeTutor")]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> BecomeTutorAsync(
            [FromBody]UpdateSettingsRequest model,
            [FromServices] ConfigurationService configurationService,
            CancellationToken token)
        {
            try
            {

                if (configurationService.GetSiteName() == ConfigurationService.Site.Frymo)
                {
                    model.Price = null;
                }
                else
                {
                    if (model.Price == null)
                    {
                        return BadRequest();
                    }
                }


                var userId = _userManager.GetLongUserId(User);
                var command = new BecomeTutorCommand(userId, model.FirstName, model.LastName,
                    model.Description, model.Bio, model.Price);
                await _commandBus.DispatchAsync(command, token);
                return Ok();
            }
            catch (ArgumentException)
            {
                return Conflict();
            }
            catch (NonUniqueObjectException)
            {
                return BadRequest();
            }
        }

        [HttpGet("content")]
        public async Task<IEnumerable<UserContentDto>> GetUserContentAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserContentByIdQuery(userId);
            var result = await _queryBus.QueryAsync(query, token);

            return result.Select(s =>
            {
                if (s is UserDocumentsDto d)
                {
                    d.Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(d.Id);
                    d.Url = Url.DocumentUrl(d.Course, d.Id, d.Name);
                }
                return s;
            });
        }

        [HttpGet("purchases")]
        public async Task<IEnumerable<UserPurchaseDto>> GetUserPurchasesAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserPurchasesByIdQuery(userId);
            var result = await _queryBus.QueryAsync(query, token);

            return result.Select(s =>
            {
                if (s is PurchasedDocumentDto d)
                {
                    d.Preview = _urlBuilder.BuildDocumentThumbnailEndpoint(d.Id);
                    d.Url = Url.DocumentUrl(d.Course, d.Id, d.Name);
                }
                return s;
            });
        }

        [HttpGet("followers")]
        public async Task<IEnumerable<FollowersDto>> GetFollowersAsync(
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserFollowersByIdQuery(userId);
            var result = await _queryBus.QueryAsync(query, token);
            return result.Select(s =>
            {
                s.Image = _urlBuilder.BuildUserImageEndpoint(s.UserId, s.Image, s.Name);
                return s;
            });
        }

        [HttpGet("calendar")]
        public async Task<DashboardCalendarDto> GetDashboardCalendarAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserCalendarByIdQuery(userId);
            var result = await _queryBus.QueryAsync(query, token);
            return result;
        }

        [HttpGet("stats")]
        [ResponseCache(Duration = TimeConst.Day, Location = ResponseCacheLocation.Client)]
        public async Task<IEnumerable<UserStatsDto>> GetTutorStatsAsync([FromQuery] UserStatsRequest request, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserStatsQuery(userId, request.Days);
            var result = await _queryBus.QueryAsync(query, token);
            return result;
        }

        [HttpGet("tutorActions")]
        public async Task<TutorActionsDto> GetTutorActionsAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new TutorActionsQuery(userId);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("questions")]
        public async Task<IEnumerable<AccountQuestionDto>> GetQuestionsAsync(
            [ProfileModelBinder(ProfileServiceQuery.Country)] UserProfile profile,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new AccountQuestionsQuery(userId, profile.Country);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("courses")]
        public async Task<IEnumerable<CourseDto>> GetCoursesAsync(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserCoursesQuery(userId);
            return await _queryBus.QueryAsync(query, token);
        }

    }
}