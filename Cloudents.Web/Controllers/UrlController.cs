using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Message;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Domain;
using Cloudents.Web.Models;

namespace Cloudents.Web.Controllers
{
    public class UrlController : Controller
    {
        private readonly IServiceBusProvider _serviceBus;
        private readonly ILogger _logger;

        public UrlController(IServiceBusProvider serviceBus, ILogger logger)
        {
            _serviceBus = serviceBus;
            _logger = logger;
        }

        private static IList<string> _domains = PrioritySource.DocumentPriority.Values
            .Union(PrioritySource.FlashcardPriority.Values)
            .SelectMany(s => s.Domains).ToList();

        public async Task<IActionResult> Index([FromQuery]UrlRequest model,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning($"url is not valid model: {model}");
                return RedirectToAction("Index", "Home");
            }
            var referer = Request.Headers["Referer"].ToString();
            var userIp = Request.HttpContext.Connection.GetIpAddress();
            var message = new UrlRedirectQueueMessage(model.Host, model.Url, referer, model.Location, userIp.ToString());
            await _serviceBus.InsertMessageAsync(message, token).ConfigureAwait(false);

            //var domains = PrioritySource.DocumentPriority.Values.Union(PrioritySource.FlashcardPriority.Values)
            //    .SelectMany(s => s.Domains);

            if (!_domains.Any(a => a.Contains(model.Url.Host, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("invalid url");
            if (model.Url.Host.Contains("courseHero", StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(
                    $"http://shareasale.com/r.cfm?b=661825&u=1469379&m=55976&urllink={model.Url.Host + model.Url.PathAndQuery + model.Url.Fragment}&afftrack=");
            }
            return Redirect(model.Url.AbsoluteUri);
        }
    }
}