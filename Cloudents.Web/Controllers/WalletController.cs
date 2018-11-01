using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Web.EventHandler;
using Cloudents.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Cloudents.Web.Binders;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class WalletController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        private readonly IDataProtect _dataProtect;
        private readonly ILogger _logger;


        public WalletController(SignInManager<User> signInManager, UserManager<User> userManager, ILogger logger, IDataProtect dataProtect)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _dataProtect = dataProtect;
            //_dataProtector = dataProtectionProvider.CreateProtector(EmailMarkAnswerAsCorrect.ProtectPurpose).ToTimeLimitedDataProtector();
        }

        // GET
        public async Task<IActionResult> Index(string code,
            [ModelBinder(typeof(CountryModelBinder))] string country)
        {
            ViewBag.country = country ?? "us";
            if (string.IsNullOrEmpty(code))
            {
                return View();
            }


            var retVal = await SignInUserAsync(code, EmailMarkAnswerAsCorrect.ProtectPurpose, _dataProtect, _userManager, _logger, _signInManager);
            if (retVal)
            {
                ViewBag.Auth = true;
            }


            return View();
        }

        public static async Task<bool> SignInUserAsync(string code, string purpose, IDataProtect dataProtector, UserManager<User> userManager,
            ILogger logger, SignInManager<User> signInManager)
        {
            try
            {

                var userId = dataProtector.Unprotect(purpose, code);
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await signInManager.SignInAsync(user, false);
                    return true;
                }
                
            }
            catch (CryptographicException ex)
            {
                //We just log the exception. user open the email too later and we can't sign it.
                //If we see this persist then maybe we need to increase the amount of time
                logger.Exception(ex);
            }
            return false;
        }
    }
}