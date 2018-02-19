using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace Cloudents.Functions
{
    [DependencyInjectionConfig(typeof(DiConfig))]
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req,
            TraceWriter log,
            [Inject()]ICommandBus bus,
            CancellationToken token)
        {

            log.Info("C# HTTP trigger function processed a request.");
            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            name = name ?? data?.name;

            var command = new CreateUrlStatsCommand("ram", DateTime.UtcNow, "ram", "ram",
                0, "127.0.0.1");

            await bus.DispatchAsync(command, token).ConfigureAwait(false);

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }
    }
}
