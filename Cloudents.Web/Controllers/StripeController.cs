using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [Authorize]
    public class StripeController : Controller
    {
        private readonly IStripeService _stripeService;
        private readonly ICommandBus _commandBus;
        private readonly UserManager<User> _userManager;

        public StripeController(IStripeService stripeService, ICommandBus commandBus, UserManager<User> userManager)
        {
            _stripeService = stripeService;
            _commandBus = commandBus;
            _userManager = userManager;
        }

        [Route("BuyPoints", Name = "stripe-buy-points"), Authorize]
        public async Task<IActionResult> StripeCallbackBuyPointsAsync(
            string redirectUrl, string sessionId,
            CancellationToken token)
        {
            var (receipt, points) = await _stripeService.GetBuyPointDataByIdAsync(sessionId,token);

            var userId = _userManager.GetLongUserId(User);
            var command = new TransferMoneyToPointsCommand(userId, points, receipt);
            await _commandBus.DispatchAsync(command, token);
            return Redirect(redirectUrl);
        }


        [Route("Subscribe", Name = "stripe-subscribe"), Authorize]
        public async Task<IActionResult> StripeCallSubscribeAsync(
            string redirectUrl, string sessionId,
            CancellationToken token)
        {
            var tutorId = await _stripeService.GetSubscriptionByIdAsync(sessionId,token);

            var userId = _userManager.GetLongUserId(User);

            var command = new SubscribeToTutorCommand(userId, tutorId);
            //var command = new TransferMoneyToPointsCommand(userId, points, receipt);
            await _commandBus.DispatchAsync(command, token);
            return Redirect(redirectUrl);
        }
        
    }
}
