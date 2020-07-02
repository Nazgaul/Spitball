using System.Collections.Generic;
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
            _logger.Info("ConfirmEmailController - getting request");
            if (!ModelState.IsValid)
            {
                
                _logger.Info("ConfirmEmailController - model not valid", new Dictionary<string, string>()
                {
                    ["id"] = model.Id.ToString(),
                    ["code"] = model.Code
                });
                return Redirect("/");
            }

            //  var code = model.Code;
            var code = System.Net.WebUtility.UrlDecode(model.Code);
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user is null)
            {
                _logger.Info("ConfirmEmailController - no user", new Dictionary<string, string>()
                {
                    ["id"] = model.Id.ToString(),
                    ["code"] = model.Code
                });
                return Redirect("/");
            }


            if (user.EmailConfirmed)
            {
                return View("ConfirmEmail");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                
                _logger.Error($"ConfirmEmailController - Error confirming email {model.Id}",new Dictionary<string, string>()
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