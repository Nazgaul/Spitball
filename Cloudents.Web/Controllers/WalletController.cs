using System.Security.Cryptography;
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
    public class WalletController : Controller
    {
        private readonly ITimeLimitedDataProtector _dataProtector;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger _logger;


        public WalletController(IDataProtectionProvider dataProtectionProvider, SignInManager<User> signInManager, UserManager<User> userManager, ILogger logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _dataProtector = dataProtectionProvider.CreateProtector(EmailMarkAnswerAsCorrect.ProtectPurpose).ToTimeLimitedDataProtector();
        }

        // GET
        public async Task<IActionResult> Index(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return View();
            }


            await SignInUserAsync(code, _dataProtector, _userManager, _logger, _signInManager);
             
            
            return View();
        }

        public static async Task SignInUserAsync(string code, ITimeLimitedDataProtector dataProtector, UserManager<User> userManager,
            ILogger logger, SignInManager<User> signInManager )
        {
            try
            {

                var userId = dataProtector.Unprotect(code);
                var user = await userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    await signInManager.SignInAsync(user, false);
                }
            }
            catch (CryptographicException ex)
            {
                //We just log the exception. user open the email too later and we can't sign it.
                //If we see this persist then maybe we need to increase the amount of time
                logger.Exception(ex);
            }
        }
    }
}