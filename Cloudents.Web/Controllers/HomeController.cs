﻿using Cloudents.Core;
using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Web.Filters;
using Cloudents.Web.Hubs;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command;
using Cloudents.Web.Models;
using Microsoft.ApplicationInsights;
using Wangkanai.Detection;

namespace Cloudents.Web.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        internal const string Referral = "referral";
        //internal const string RootRoute = "Root";
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;

        public HomeController(SignInManager<User> signInManager, ILogger logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        [ResponseCache(Location = ResponseCacheLocation.Client, Duration = TimeConst.Hour, NoStore = true), SignInWithToken]
        [ApiNotFoundFilter]
        //[Route("", Name = RootRoute)]
        public IActionResult Index(
            [FromServices] Lazy<ICrawlerResolver> crawlerResolver,
            [FromHeader(Name = "User-Agent")] string userAgent,
            [FromQuery, CanBeNull] string referral,
            [FromQuery] string open
            )
        {
            if (!string.IsNullOrEmpty(referral))
            {
                TempData[Referral] = referral;
            }

            try
            {
                if (crawlerResolver.Value.Crawler?.Type == CrawlerType.LinkedIn)
                {
                    ViewBag.fbImage = ViewBag.imageSrc = "/images/3rdParty/linkedinShare.png";
                }
            }
            catch (Exception ex)
            {

                _logger.Exception(ex, new Dictionary<string, string>()
                {
                    ["userAgent"] = userAgent
                });
            }

            if (_signInManager.IsSignedIn(User))
            {
                return View();
            }

            if (open?.Equals("referral", StringComparison.OrdinalIgnoreCase) == true)
            {
                return RedirectToAction("Index", new
                {
                    referral
                });
            }


            return View();
        }

        [Route("logout")]
        public async Task<IActionResult> LogOutAsync(
            [FromServices] SignInManager<User> signInManager,
            [FromServices] IHubContext<SbHub> hubContext, CancellationToken token)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }
            var message = new SignalRTransportType(SignalRType.User, SignalREventAction.Logout,
                new object());

            await hubContext.Clients.User(signInManager.UserManager.GetUserId(User)).SendCoreAsync("Message", new object[]
            {
                message
            }, token);
            await signInManager.SignOutAsync();
            TempData.Clear();


            return Redirect("/");
        }



        [Route("image/{hash}", Name = "imageUrl")]
        [ResponseCache(
            Duration = TimeConst.Month, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "*" })]
        public IActionResult ImageRedirect([FromRoute]string hash, [FromServices] IConfiguration configuration)

        {
            string val = Request.QueryString.ToUriComponent();
            if (!val.StartsWith("?"))
            {
                val = $"?{val}";
            }
            return Redirect(
                $"{configuration["functionCdnEndpoint"]}/api/image/{hash}{val}");
        }


        [Route("PaymentProcessing", Name = "ReturnUrl")]
        public async Task<IActionResult> Processing( PaymeSuccessCallback model,
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
                values.Add("userId",model.UserId.ToString());
                logger.TrackTrace("Credit Card Process Failed", values);
            }
            return View("Processing", model);
        }


    }
}