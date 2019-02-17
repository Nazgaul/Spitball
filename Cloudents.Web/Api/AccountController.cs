using Cloudents.Web.Extensions;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Query;
using Cloudents.Query.Query;
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
        private readonly IConfiguration _configuration;
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;


        public AccountController(UserManager<RegularUser> userManager,
            SignInManager<RegularUser> signInManager, IConfiguration configuration, ICommandBus commandBus, IQueryBus queryBus)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
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
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserDataByIdQuery(userId);
            var taskUser = queryBus.QueryAsync<UserAccountDto>(query, token);
            var talkJs = GetToken();

            var user = await taskUser;

            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return Unauthorized();
            }

            if (user.Score != score)
            {
                var regularUser = await _userManager.GetUserAsync(User);
                await _signInManager.RefreshSignInAsync(regularUser);
            }
            user.Token = talkJs;
            return user;
        }

        private string GetToken()
        {
            var message = _userManager.GetUserId(User);

            var asciiEncoding = new ASCIIEncoding();
            var keyByte = asciiEncoding.GetBytes(_configuration["TalkJsSecret"]);
            var messageBytes = asciiEncoding.GetBytes(message);

            using (var sha256 = new HMACSHA256(keyByte))
            {
                var hashMessage = sha256.ComputeHash(messageBytes);

                var result = new StringBuilder();
                foreach (byte b in hashMessage)
                {
                    result.Append(b.ToString("X2"));
                }
                return result.ToString();
            }
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
        public async Task<IEnumerable<CourseDto>> GetCourses(
            CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserProfileQuery(userId);
            var t = await _queryBus.QueryAsync(query, token);
            return t.Courses.Select(s => new CourseDto(s));
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
            //TODO - should be user profile query
            var query = new UniversityQuery(universityId.Value);
            return await _queryBus.QueryAsync(query, token);
        }

        [HttpGet("{id}/referrals")]
        public async Task<UserReferralsDto> GetReferrals(CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserReferralsQuery(userId);
            return await _queryBus.QueryAsync(query, token);
        }

    }
}