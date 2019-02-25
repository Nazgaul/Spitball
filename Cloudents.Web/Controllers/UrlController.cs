using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Cloudents.Web.Models;

namespace Cloudents.Web.Controllers
{
    [Route("[controller]", Name = "Url")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UrlController : Controller
    {
        private readonly IQueueProvider _queueProvider;
        private readonly ILogger _logger;
        //private readonly IDomainParser _domainParser;
       

        public UrlController(IQueueProvider queueProvider, ILogger logger)
        {
            _queueProvider = queueProvider;
            _logger = logger;
        }

        private static readonly IList<string> Domains = PrioritySource.DocumentPriority.Values
            .Union(PrioritySource.FlashcardPriority.Values)
            .SelectMany(s => s.Domains).ToList();

        public IActionResult Index([FromQuery]UrlRequest model,
            CancellationToken token)
        {
            if (!ModelState.IsValid)
            {
                _logger.Warning($"url is not valid model: {model}");
                return RedirectToAction("Index", "Home");
            }

            if (!model.Url.IsAbsoluteUri)
            {
                _logger.Warning($"url is not valid model: {model}");
                return RedirectToAction("Index", "Home");
            }

            var referer = Request.Headers["Referer"].ToString();
            var userIp = Request.HttpContext.Connection.GetIpAddress();


            if (!Uri.TryCreate(referer, UriKind.Absolute, out var refererUri))
            {
                return RedirectToAction("Index", "Home");
            }

            //if (refererUri.PathAndQuery.Contains(new[] { "flashcard", "note" }, StringComparison.OrdinalIgnoreCase))
            //{
                //var domain = _domainParser.GetDomain(model.Url.Host);
                //if (!Domains.Any(a => a.Contains(domain, StringComparison.OrdinalIgnoreCase)))
                //    throw new ArgumentException("invalid url");
            //}
            _queueProvider.InsertMessageAsync(new RedirectUserMessage(model.Host, model.Url, referer, model.Location, userIp.ToString()), token);

            if (model.Url.Host.Contains("courseHero", StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(
                    $"http://shareasale.com/r.cfm?b=661825&u=1469379&m=55976&urllink={model.Url.Host + model.Url.PathAndQuery + model.Url.Fragment}&afftrack=");
            }
            return Redirect(model.Url.AbsoluteUri);
        }
    }
}