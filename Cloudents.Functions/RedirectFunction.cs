using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Storage;
using JetBrains.Annotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Cloudents.Functions
{
    public static class RedirectFunction
    {
        //[FunctionName("UrlRedirect")]
        //[UsedImplicitly]
        //public static async Task<HttpResponseMessage> RunAsync(
        //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "redirect")]HttpRequestMessage req,
        //    [Queue(QueueName.UrlRedirectName)] IAsyncCollector<UrlRedirectQueueMessage> queue,
        //    CancellationToken token)
        //{
        //    var referrer = req.Headers.Referrer?.ToString();
        //    var queryString = req.GetQueryNameValuePairs().ToList();
        //    var host = queryString.Find(f => f.Key == "host").Value;
        //    int? location = null;
        //    if (int.TryParse(queryString.Find(f => f.Key == "location").Value, out var locationInt))
        //    {
        //        location = locationInt;
        //    }
        //    var url = queryString.Find(f => f.Key == "url").Value;
        //    var userIp = req.GetClientIpAddress();
        //    var message = new UrlRedirectQueueMessage(host, url, referrer, location, userIp);
        //    await queue.AddAsync(message, token).ConfigureAwait(false);

        //    var res = req.CreateResponse(HttpStatusCode.Redirect);
        //    res.Headers.Add("Location", url);
        //    return res;
        //}

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

        


       
    }
}
