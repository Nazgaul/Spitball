using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.EventHandler;
using Cloudents.Core.Interfaces;
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
        private readonly ILogger _logger;



        public QuestionController(IDataProtectionProvider dataProtectionProvider, SignInManager<User> signInManager, UserManager<User> userManager, ILogger logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
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
            await WalletController.SignInUserAsync(code, _dataProtector, _userManager, _logger, _signInManager);
            return View();
        }
    }
}