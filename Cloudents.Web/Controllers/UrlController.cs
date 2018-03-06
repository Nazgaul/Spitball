using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
using Cloudents.Web.Extensions;
using Cloudents.Web.Extensions.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    [Obsolete("will be removed in v13")]
    public class UrlController : Controller
    {
        private readonly IQueueProvider _queueProvider;

        public UrlController(IQueueProvider queueProvider)
        {
            _queueProvider = queueProvider;
        }

        public async Task<IActionResult> Index(string host,
            string url,
            int location,
            CancellationToken token)
        {
            var referer = Request.Headers["Referer"].ToString();
            var userIp = Request.HttpContext.Connection.GetIpAddress();
            var message = new UrlRedirectQueueMessage(host, url, referer, location, userIp.ToString());
            await _queueProvider.InsertMessageAsync(QueueName.UrlRedirect, message, token).ConfigureAwait(false);
            return Redirect(url);
        }
    }
}