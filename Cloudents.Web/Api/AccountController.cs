using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using NHibernate;
using System.Drawing;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<RegularUser> _userManager;
        private readonly SignInManager<RegularUser> _signInManager;
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public AccountController(UserManager<RegularUser> userManager,
            SignInManager<RegularUser> signInManager, ICommandBus commandBus, IQueryBus queryBus)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _commandBus = commandBus;
            _queryBus = queryBus;
        }

        // GET
        [HttpGet]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status401Unauthorized)]
        [ProducesResponseType(Status200OK)]
        public async Task<ActionResult<UserAccountDto>> GetAsync(
            [FromServices] IQueryBus queryBus,
            [ClaimModelBinder(AppClaimsPrincipalFactory.Score)] int score,
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

            if (user.Score != score)
            {
                var regularUser = await _userManager.GetUserAsync(User);
                await _signInManager.RefreshSignInAsync(regularUser);
            }
            return user;
        }

        [AllowAnonymous, HttpPost("language")]
        public async Task<IActionResult> ChangeLanguage([FromBody]ChangeCultureRequest model, CancellationToken token)
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


        /// <summary>
        /// Perform course search per user
        /// </summary>
        /// <param name="token"></param>
        /// <returns>list of courses for a user</returns>
        [HttpGet("courses")]
        public async Task<IEnumerable<CourseDto>> GetCourses(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);

            var query = new UserCoursesQuery(userId);
            var result = await _queryBus.QueryAsync(query, token);
            return result;
        }

        [HttpGet("University")]
        public async Task<UniversityDto> GetUniversityAsync(
            [ClaimModelBinder(AppClaimsPrincipalFactory.University)] Guid? universityId,
            CancellationToken token)
        {
            if (!universityId.HasValue)
            {
                return null;
            }
            var query = new UniversityQuery(universityId.Value);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("referrals")]
        [ResponseCache(Duration = TimeConst.Minute * 30, Location = ResponseCacheLocation.Client)]
        public async Task<UserReferralsDto> GetReferrals(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserReferralsQuery(userId);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImageAsync(IFormFile file,
            [FromServices] IUserDirectoryBlobProvider blobProvider,
            [FromServices] UserManager<RegularUser> userManager,
            [FromServices] IBinarySerializer serializer,
            CancellationToken token)
        {
            if (file == null)
            {
                ModelState.AddModelError("x", "no file");
                return BadRequest(ModelState);
            }

            string[] supportedImages = { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };
            if (!file.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("x","not an image");
                return BadRequest(ModelState);
            }


            var extension = Path.GetExtension(file.FileName);

            if (!supportedImages.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("x", "not an image");
                return BadRequest(ModelState);
            }
            try
            {
                using (var x = Image.FromStream(file.OpenReadStream()))
                {

                }
            }
            catch
            {
                ModelState.AddModelError("x", "unsupported format");
                return BadRequest(ModelState);
            }
            var userId = userManager.GetLongUserId(User);
            var fileName = $"{userId}/{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{extension}";
            await blobProvider.UploadStreamAsync(fileName, file.OpenReadStream(),
                file.ContentType, TimeSpan.FromDays(365), token);

            var fileUri = blobProvider.GetBlobUrl(fileName);
            var imageProperties = new ImageProperties(fileUri, ImageProperties.BlurEffect.None);

            var hash = serializer.Serialize(imageProperties);
            var url = Url.RouteUrl("imageUrl", new
            {
                hash = Base64UrlTextEncoder.Encode(hash)
            });
            var command = new UpdateUserImageCommand(userId, url);
            await _commandBus.DispatchAsync(command, token);
            return Ok(url);
        }

        [HttpPost("settings")]
        public async Task<IActionResult> ChangeSettingsAsync([FromBody]UpdateSettingsRequest model,
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var command = new UpdateUserSettingsCommand(userId, model.FirstName, model.LastName,
                model.Description, model.Bio, model.Price);
            await _commandBus.DispatchAsync(command, token);
            return Ok();
        }

        [HttpPost("BecomeTutor")]
        public async Task<IActionResult> BecomeTutorAsync(
            [FromBody]UpdateSettingsRequest model, CancellationToken token)
        {
            try
            {
                if (model.Price == null)
                {
                    return BadRequest();
                }

                var userId = _userManager.GetLongUserId(User);
                var command = new BecomeTutorCommand(userId, model.FirstName, model.LastName,
                    model.Description, model.Bio, model.Price.GetValueOrDefault());
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




    }
}