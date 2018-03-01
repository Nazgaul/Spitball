using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Cloudents.Core.Extension;

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
    public static class RedirectFunction
    {
        [FunctionName("UrlRedirect")]
        [UsedImplicitly]
        public static async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "redirect")]HttpRequestMessage req,
            [Queue(QueueName.UrlRedirectName)] IAsyncCollector<UrlRedirectQueueMessage> queue,
            CancellationToken token)
        {
            var referrer = req.Headers.Referrer?.ToString();
            var queryString = req.GetQueryNameValuePairs().ToList();
            var host = queryString.Find(f => f.Key == "host").Value;
            var location = int.Parse(queryString.Find(f => f.Key == "location").Value);
            var url = queryString.Find(f => f.Key == "url").Value;

            if (host.Contains("studyblue", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(url);

                var uriBuilder = new UriBuilder(uri)
                {
                    Query = "?utm_source=spitball&utm_medium=referral"
                };
                url = uriBuilder.ToString();
            }

            var userIp = req.GetClientIpAddress();
            var message = new UrlRedirectQueueMessage(host, url, referrer, location, userIp);
            await queue.AddAsync(message, token).ConfigureAwait(false);

            var res = req.CreateResponse(HttpStatusCode.Redirect);
            res.Headers.Add("Location", url);
            return res;
        }

        [FunctionName("UrlProcess")]
        [UsedImplicitly]
        public static async Task ProcessQueueMessage([QueueTrigger(QueueName.UrlRedirectName)] UrlRedirectQueueMessage content,
            TraceWriter log, CancellationToken token,[Inject] ICommandBus commandBus)
        {
            var command = new CreateUrlStatsCommand(content.Host, content.DateTime, content.Url, content.UrlReferrer,
                content.Location, content.Ip);

            await commandBus.DispatchAsync(command, token).ConfigureAwait(false);
             log.Info("Finish Process");
        }
    }
}
