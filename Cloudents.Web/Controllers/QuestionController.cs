using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.EventHandler;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class QuestionController : Controller
    {
        private readonly ITimeLimitedDataProtector _dataProtector;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;


        public QuestionController(IDataProtectionProvider dataProtectionProvider, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dataProtector = dataProtectionProvider.CreateProtector(EmailAnswerCreated.CreateAnswer).ToTimeLimitedDataProtector();
        }
        // GET
        [Route("[controller]/{id:long}")]
        public async Task<IActionResult> Index(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return View();
            }

            var userId = _dataProtector.Unprotect(code);
            //if (long.TryParse(userIdStr, out var userId))
            // {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _signInManager.SignInAsync(user, false);
            }

            return RedirectToAction("Index");
            //}


        }
    }
}