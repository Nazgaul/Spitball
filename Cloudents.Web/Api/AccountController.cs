using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ICommandBus _commandBus;


        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager, IConfiguration configuration, ICommandBus commandBus)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _commandBus = commandBus;
        }

        // GET
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<UserAccountDto>> GetAsync([FromServices] IQueryBus queryBus, CancellationToken token)
        {
            var userId = _userManager.GetLongUserId(User);
            var query = new UserDataByIdQuery(userId);
            var taskUser = queryBus.QueryAsync<UserAccountDto>(query, token);
            var talkJs = GetToken();

            var user = await taskUser.ConfigureAwait(false);
            if (user == null)
            {
                await _signInManager.SignOutAsync().ConfigureAwait(false);
                return Unauthorized();
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
            var command = new UpdateUserCultureCommand(userId, culture.Culture);
            await _commandBus.DispatchAsync(command, token);

            return Ok();
        }


    }
}