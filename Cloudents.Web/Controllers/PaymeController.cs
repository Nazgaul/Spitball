using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Core.Entities;
using Cloudents.Web.Api;
using Cloudents.Web.Extensions;
using Cloudents.Web.Models;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PaymeController : Controller
    {
        // GET
     

        private readonly UserManager<User> _userManager;
        private readonly ICommandBus _commandBus; 

        public PaymeController(UserManager<User> userManager, ICommandBus commandBus)
        {
            _userManager = userManager;
            _commandBus = commandBus;
        }

        public const string EnrollStudyRoom = "payme-enroll-study-room";
        internal const string PaymeCallbackRouteName = "ReturnUrl";
        //internal const string PaymeCallbackRouteName2 = "ReturnUrl2";
        [Route("PaymentProcessing", Name = PaymeCallbackRouteName)]
        public async Task<IActionResult> Processing(PaymeSuccessCallback model,
            [FromServices] ICommandBus commandBus,
            [FromServices] TelemetryClient logger,
            CancellationToken token)
        {
            if (model.Status.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                var command = new ConfirmPaymentCommand(model.UserId);
                await commandBus.DispatchAsync(command, token);
            }
            else
            {
                var values = Request.Form.ToDictionary(s => s.Key, x => x.Value.ToString());
                values.Add("userId", model.UserId.ToString());
                logger.TrackTrace("Credit Card Process Failed", values);
            }
            return View("Processing", model);
        }


        [Route("IL-Enroll", Name = EnrollStudyRoom)]
        public async Task<IActionResult> PaymeCallbackEnrollAsync(
            PaymeSuccessCallback model,
            [FromQuery] long courseId, 
            [FromQuery] string redirectUrl, 
            CancellationToken token)
        {

            if (model.Status.Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                
                var userId = _userManager.GetLongUserId(User);
                var command = new CourseEnrollCommand(userId, courseId, model.TransactionId);
                await _commandBus.DispatchAsync(command, token);
                return Redirect(redirectUrl);
            }

            //var metaData = await _stripeService.GetMetaDataAsync(sessionId, token);

            //var courseId = long.Parse(metaData[WalletController.StudyroomIdMetaData]);

            //var userId = _userManager.GetLongUserId(User);
            //var command = new CourseEnrollCommand(userId, courseId, sessionId);
            //await _commandBus.DispatchAsync(command, token);
            return Redirect(redirectUrl);
        }
    }
}