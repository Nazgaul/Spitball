using Cloudents.Domain.Entities;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cloudents.Application.Interfaces;
using EmailAnswerCreated = Cloudents.Web.EventHandler.EmailAnswerCreated;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class QuestionController : Controller
    {
        private readonly SignInManager<RegularUser> _signInManager;
        private readonly UserManager<RegularUser> _userManager;
        private readonly ILogger _logger;
        private readonly IDataProtect _dataProtect;




        public QuestionController(SignInManager<RegularUser> signInManager, UserManager<RegularUser> userManager, ILogger logger, IDataProtect dataProtect)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _dataProtect = dataProtect;
            // _dataProtector = dataProtectionProvider.CreateProtector(EmailAnswerCreated.CreateAnswer).ToTimeLimitedDataProtector();
        }
        // GET
        [Route("[controller]/{id:long}")]
        public async Task<IActionResult> Index(string code
            /*[ModelBinder(typeof(CountryModelBinder))] string country*/)
        {
            //ViewBag.country = country ?? "us";
            if (string.IsNullOrEmpty(code))
            {
                return View();
            }
            var retVal = await WalletController.SignInUserAsync(code, EmailAnswerCreated.CreateAnswer, _dataProtect, _userManager, _logger, _signInManager);
            if (retVal)
            {
                ViewBag.Auth = true;
            }
            return View();
        }
    }
}