using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Web.Filters;
using Cloudents.Web.Identity;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize(Policy = SignInStep.PolicyAll)]

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

            return Ok(new
            {
                user.Id,
                user.Image,
                user.Email,
                user.Name,
                token = GetToken()
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

        [HttpGet("userName")]
        [Authorize(Policy = SignInStep.PolicyPassword)]
        public IActionResult GetUserName()
        {
            var name = _userManager.GetUserName(User);
            return Ok(new { name });
        }

        [HttpPost("userName"), ValidateModel]
        [Authorize(Policy = SignInStep.PolicyPassword)]
        public async Task<IActionResult> ChangeUserNameAsync([FromBody]ChangeUserNameRequest model)
        {
            //TODO: check if this unique
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            var result = await _userManager.SetUserNameAsync(user, model.Name).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest();
        }

    }
}