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
        private readonly IStripeService stripeService;
        private readonly ICommandBus commandBus;
        private readonly UserManager<User> userManager;

        public StripeController(IStripeService stripeService, ICommandBus commandBus, UserManager<User> userManager)
        {
            this.stripeService = stripeService;
            this.commandBus = commandBus;
            this.userManager = userManager;
        }

        [Route("BuyPoints", Name = "stripe-buy-points"), Authorize]
        public async Task<IActionResult> StripeCallbackBuyPointsAsync(
            string redirectUrl, string sessionId,
            CancellationToken token)
        {
            var (receipt, points) = await stripeService.GetBuyPointDataByIdAsync(sessionId,token);

            var userId = userManager.GetLongUserId(User);
            var command = new TransferMoneyToPointsCommand(userId, points, receipt);
            await commandBus.DispatchAsync(command, token);
            return Redirect(redirectUrl);
        }


        [Route("Subscribe", Name = "stripe-subscribe"), Authorize]
        public async Task<IActionResult> StripeCallSubscribeAsync(
            string redirectUrl, string sessionId,
            CancellationToken token)
        {
            var tutorId = await stripeService.GetSubscriptionByIdAsync(sessionId,token);

            var userId = userManager.GetLongUserId(User);

            var command = new SubscribeToTutorCommand(userId, tutorId);
            //var command = new TransferMoneyToPointsCommand(userId, points, receipt);
            await commandBus.DispatchAsync(command, token);
            return Redirect(redirectUrl);
        }
    }
}
