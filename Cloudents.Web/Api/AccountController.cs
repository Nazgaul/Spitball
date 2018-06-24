using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Cloudents.Core.Command;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<User> userManager, ICommandBus commandBus, IMapper mapper, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _commandBus = commandBus;
            _mapper = mapper;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // GET
        [HttpGet]
        [Authorize]

        public async Task<IActionResult> GetAsync(CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty,"user not exists");
                await _signInManager.SignOutAsync().ConfigureAwait(false);
                return BadRequest(ModelState);
            }

            //var user = taskUser.Result;
            return Ok(new
            {
                user.Id,
                user.Image,
                user.Email,
                user.Name,
                token = GetToken(),
                dollar = user.Balance / 40,
                balance = user.Balance
            });
        }

        private string GetToken()
        {
            // ReSharper disable once StringLiteralTypo
           // const string key = _configuration["TalkJsSecret"];// "sk_test_AQGzQ2Rlj0NeiNOEdj1SlosU";
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

        //TODO : need to figure out what well do.
        [HttpPost("university")]
        [Authorize]
        public async Task<IActionResult> AssignUniversityAsync([FromBody] AssignUniversityRequest model, CancellationToken token)
        {
            var command = _mapper.Map<AssignUniversityToUserCommand>(model);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogOutAsync()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}