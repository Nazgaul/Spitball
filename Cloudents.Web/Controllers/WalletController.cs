//using Cloudents.Web.EventHandler;
//using Cloudents.Web.Services;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Cryptography;
//using System.Threading.Tasks;
//using Cloudents.Core.Entities;
//using Cloudents.Core.Interfaces;

//namespace Cloudents.Web.Controllers
//{
//    [ApiExplorerSettings(IgnoreApi = true)]
//    public class WalletController : Controller
//    {
//        private readonly SignInManager<RegularUser> _signInManager;
//        private readonly UserManager<RegularUser> _userManager;

//        private readonly IDataProtect _dataProtect;
//        private readonly ILogger _logger;


//        public WalletController(SignInManager<RegularUser> signInManager, UserManager<RegularUser> userManager, ILogger logger, IDataProtect dataProtect)
//        {
//            _signInManager = signInManager;
//            _userManager = userManager;
//            _logger = logger;
//            _dataProtect = dataProtect;
//            //_dataProtector = dataProtectionProvider.CreateProtector(EmailMarkAnswerAsCorrect.ProtectPurpose).ToTimeLimitedDataProtector();
//        }

//        // GET
//        public async Task<IActionResult> Index(string code)
//        {
//            //ViewBag.country = country ?? "us";
//            if (string.IsNullOrEmpty(code))
//            {
//                return View();
//            }


//            var retVal = await SignInUserAsync(code, EmailMarkAnswerAsCorrect.ProtectPurpose, _dataProtect, _userManager, _logger, _signInManager);
//            if (retVal)
//            {
//                ViewBag.Auth = true;
//            }


//            return View();
//        }

       
//    }
//}