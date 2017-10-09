using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Cloudents.Infrastructure
{
    public interface IRestClient
    {
        Task<JObject> GetAsync(Uri url, NameValueCollection queryString, CancellationToken token);
    }
}