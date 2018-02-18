using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Cloudents.WebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        //public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        //{
        //    log.WriteLine(message);
        //}
        private readonly ICommandBus _commandBus;

        public Functions(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        [FunctionName("UrlProcess")]
        public async Task ProcessQueueMessage([QueueTrigger(QueueName.UrlRedirectName)] UrlRedirectQueueMessage content, TextWriter log, CancellationToken token)
        {
            var command = new CreateUrlStatsCommand(content.Host, content.DateTime, content.Url, content.UrlReferrer,
                content.Location, content.Ip);

            await _commandBus.DispatchAsync(command, token).ConfigureAwait(false);
            await log.WriteLineAsync("Finish Process").ConfigureAwait(false);
        }

        [FunctionName("Redirect")]
        public async Task<HttpResponseMessage> ProcessRedirectAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get",Route="redirect")]
            HttpRequestMessage req, TextWriter log)
        {
            await log.WriteLineAsync("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            name = name ?? data?.name;

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }
    }
}
