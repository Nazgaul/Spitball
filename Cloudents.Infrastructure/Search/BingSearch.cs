using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Microsoft.Azure.CognitiveServices.Search.CustomSearch;

namespace Cloudents.Infrastructure.Search
{
    public class BingSearch : ISearch, IDisposable
    {
        private readonly ICustomSearchAPI _api;
        public BingSearch()
        {
            var x = new ApiKeyServiceClientCredentials("285e26627c874d28be01859b4fb08a58");
            _api = new CustomSearchAPI(x);
        }
        public async Task<IEnumerable<SearchResult>> DoSearchAsync(CseModel model, CancellationToken token)
        {
            //const int  config = 2506829495;
            var result = await _api.CustomInstance.SearchAsync("microsoft");
            return result.WebPages.Value.Select(s => new SearchResult()
            {
                Url = s.Url,
                Id = s.BingId,
                //   Image = s.
            });


        }

        public void Dispose()
        {
            _api?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
