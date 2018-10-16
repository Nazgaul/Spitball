using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EmailAnswerCreated = Cloudents.Web.EventHandler.EmailAnswerCreated;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class QuestionController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;
        private readonly IDataProtect _dataProtect;




        public QuestionController(SignInManager<User> signInManager, UserManager<User> userManager, ILogger logger, IDataProtect dataProtect)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _dataProtect = dataProtect;
            // _dataProtector = dataProtectionProvider.CreateProtector(EmailAnswerCreated.CreateAnswer).ToTimeLimitedDataProtector();
        }
        // GET
        [Route("[controller]/{id:long}")]
        public async Task<IActionResult> Index(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return View();
            }
            await WalletController.SignInUserAsync(code, EmailAnswerCreated.CreateAnswer, _dataProtect, _userManager, _logger, _signInManager);
            return View();
        }
    }
}