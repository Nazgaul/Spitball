using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Cloudents.Functions.Di;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Functions
{
    public static class RedirectFunction
    {
        [FunctionName("UrlRedirect")]
        [UsedImplicitly]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "redirect")]HttpRequest req,
            [Queue(QueueName.UrlRedirectName)] IAsyncCollector<UrlRedirectQueueMessage> queue,
            CancellationToken token)
        {
            var referrer = req.Headers["Referrer"];
            var host = req.Query["host"].ToString();
            int? location = null;
            if (int.TryParse(req.Query["location"].ToString(), out var locationInt))
            {
                location = locationInt;
            }
            var url = req.Query["url"].ToString();
            var userIp = req.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var message = new UrlRedirectQueueMessage(host, url, referrer, location, userIp);
            await queue.AddAsync(message, token).ConfigureAwait(false);

            return new RedirectResult(url);
        }

        //private static readonly Dictionary<string, Func<string, string>> HostFuncs = new Dictionary<string, Func<string, string>>(StringComparer.OrdinalIgnoreCase)
        //{
        //    {
        //        "studyBlue" , url =>
        //        {
        //            var uri = new Uri(url);
        //            var uriBuilder = new UriBuilder(uri)
        //            {
        //                Query = "?utm_source=spitball&utm_medium=referral"
        //            };
        //            return uriBuilder.ToString();
        //        }
        //    }
        //};

        [FunctionName("UrlProcess")]
        [UsedImplicitly]
        public static async Task ProcessQueueMessageAsync([QueueTrigger(QueueName.UrlRedirectName)] UrlRedirectQueueMessage content,
            TraceWriter log, CancellationToken token, [Inject] ICommandBus commandBus)
        {
            var command = new CreateUrlStatsCommand(content.Host, content.DateTime, content.Url, content.UrlReferrer,
                content.Location, content.Ip);

            await commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            log.Info("Finish Process");


        }


       
    }
}
