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

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ICommandBus _commandBus;
        private readonly IMapper _mapper;

        public AccountController(UserManager<User> userManager, ICommandBus commandBus, IMapper mapper)
        {
            _userManager = userManager;
            _commandBus = commandBus;
            _mapper = mapper;
        }

        // GET
        [HttpGet]
        [Authorize(Policy = PolicyType.Finish)]

        public async Task<IActionResult> GetAsync(
            [FromServices] IBlockChainErc20Service blockChain, CancellationToken token)
        {
            var publicKeyClaim = User.Claims.First(f => f.Type == ClaimsType.PublicKey);

            var taskUser = _userManager.GetUserAsync(User);
            var taskBalance = blockChain.GetBalanceAsync(publicKeyClaim.Value, token);

            await Task.WhenAll(taskUser, taskBalance).ConfigureAwait(false);

            var user = taskUser.Result;
            return Ok(new
            {
                user.Id,
                user.Image,
                user.Email,
                user.Name,
                token = GetToken(),
                balance = taskBalance.Result
            });
        }

        private string GetToken()
        {
            // ReSharper disable once StringLiteralTypo
            const string key = "sk_test_AQGzQ2Rlj0NeiNOEdj1SlosU";
            var message = _userManager.GetUserId(User);

            var asciiEncoding = new ASCIIEncoding();
            var keyByte = asciiEncoding.GetBytes(key);
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
        [Authorize(Policy = PolicyType.Finish)]
        public async Task<IActionResult> AssignUniversityAsync([FromBody] AssignUniversityRequest model, CancellationToken token)
        {
            var command = _mapper.Map<AssignUniversityToUserCommand>(model);
            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> LogOutAsync(
            [FromServices] SignInManager<User> signInManager)
        {
            await signInManager.SignOutAsync().ConfigureAwait(false);
            return Ok();
        }
    }
}