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
using Cloudents.Infrastructure.Domain;

namespace Cloudents.Web.Controllers
{
    public class UrlController : Controller
    {
        private readonly IServiceBusProvider _serviceBus;

        public UrlController(IServiceBusProvider serviceBus, IDomainParser domainParser)
        {
            _serviceBus = serviceBus;
        }

        private static IList<string> _domains = PrioritySource.DocumentPriority.Values
            .Union(PrioritySource.FlashcardPriority.Values)
            .SelectMany(s => s.Domains).ToList();

        public async Task<IActionResult> Index(string host,
            Uri url,
            int location,
            CancellationToken token)
        {
            var referer = Request.Headers["Referer"].ToString();
            var userIp = Request.HttpContext.Connection.GetIpAddress();
            var message = new UrlRedirectQueueMessage(host, url, referer, location, userIp.ToString());
            await _serviceBus.InsertMessageAsync(message, token).ConfigureAwait(false);

            //var domains = PrioritySource.DocumentPriority.Values.Union(PrioritySource.FlashcardPriority.Values)
            //    .SelectMany(s => s.Domains);

            if (!_domains.Any(a => a.Contains(url.Host, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("invalid url");

            

            if (url.Host.Contains("courseHero", StringComparison.OrdinalIgnoreCase))
            {
                return Redirect(
                    $"http://shareasale.com/r.cfm?b=661825&u=1469379&m=55976&urllink={url.Host + url.PathAndQuery + url.Fragment}&afftrack=");
            }
            return Redirect(url.AbsoluteUri);
        }
    }
}