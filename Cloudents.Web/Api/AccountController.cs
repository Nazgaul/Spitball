using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize, ApiController]

    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        //private readonly ICommandBus _commandBus;
        //private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<User> userManager, /*ICommandBus commandBus, IMapper mapper,*/
            SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
           // _commandBus = commandBus;
           // _mapper = mapper;
            _signInManager = signInManager;
            _configuration = configuration;
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
                //TODO: Localize
                ModelState.AddModelError(string.Empty,"user not exists");
                await _signInManager.SignOutAsync().ConfigureAwait(false);
                return BadRequest(ModelState);
            }
            user.Token = talkJs;
            return user;
        }

        private string GetToken()
        {
            // ReSharper disable once StringLiteralTypo
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

        //[HttpPost("university")]
        //public async Task<IActionResult> AssignUniversityAsync([FromBody] AssignUniversityRequest model, CancellationToken token)
        //{
        //    var command = _mapper.Map<AssignUniversityToUserCommand>(model);
        //    await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
        //    return Ok();
        //}
    }
}