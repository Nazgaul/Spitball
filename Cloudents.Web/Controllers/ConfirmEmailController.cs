using System.Collections.Generic;
using System.Text.Encodings.Web;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("[controller]", Name = "ConfirmEmail")]
    public class ConfirmEmailController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;


        public ConfirmEmailController(UserManager<User> userManager, ILogger logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        // GET
        public async Task<IActionResult> IndexAsync(ConfirmEmailRequest model, CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                return Redirect("/");
            }

            //  var code = model.Code;
            var code = System.Net.WebUtility.UrlDecode(model.Code);
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                _logger.Error("Confirm user email is null");
                return Redirect("/");
            }


            if (user.EmailConfirmed)
            {
                return View("ConfirmEmail");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                
                _logger.Error($"Error confirming email {model.Id}",new Dictionary<string, string>()
                {
                    ["UserId"] = model.Id.ToString(),
                    ["result"] = code,
                    ["Encoded"] = model.Code
                }); 
                return Redirect("/");

            }


            return View("ConfirmEmail");
        }


    }
}