using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Controllers
{
    public class UrlController : Controller
    {
        private readonly ICommandBus _bus;

        public UrlController(ICommandBus bus)
        {
            _bus = bus;
        }
        //private readonly IRestClient _client;
        //private readonly IQueueProvider _queueProvider;

        //public UrlController(IRestClient client, IQueueProvider queueProvider)
        //{
        //    _client = client;
        //    _queueProvider = queueProvider;
        //}

        public async Task<IActionResult> Index(string host, string url,int location, CancellationToken token)
        {
            var referer = Request.Headers["Referer"].ToString();
            var message = new UrlRedirectQueueMessage(host, url, referer, location);

            var command = new CreateUrlStatsCommand(host, DateTime.UtcNow, url, referer,
                location);

            await _bus.DispatchAsync(command, token).ConfigureAwait(false);

            //await _queueProvider.InsertMessageAsync(QueueName.UrlRedirect, message, token).ConfigureAwait(false);
            //var p = await _client.UrlRedirectAsync(new Uri(url)).ConfigureAwait(false);
            return Redirect(url);
        }
    }
}