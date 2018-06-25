using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : Controller
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly UserManager<User> _userManager;

        public WalletController(ITransactionRepository transactionRepository, UserManager<User> userManager)
        {
            _transactionRepository = transactionRepository;
            _userManager = userManager;
        }

        // GET
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalanceAsync(CancellationToken token)
        {
            var userId = long.Parse(_userManager.GetUserId(User));
            var retVal = await _transactionRepository.GetCurrentBalanceDetailAsync(userId, token).ConfigureAwait(false);

            return Ok(retVal);
        }


        [HttpGet("balance")]
        public async Task<IActionResult> GetTransactionAsync(CancellationToken token)
        {
            var userId = long.Parse(_userManager.GetUserId(User));
            var retVal = await _transactionRepository.GetTransactionsAsync(userId, token).ConfigureAwait(false);

            return Ok(retVal);
        }
    }
}