using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IRestClient
    {
     

        Task<(Stream stream, EntityTagHeaderValue etagHeader)> DownloadStreamAsync(Uri url,
            CancellationToken token);
      
    }
}