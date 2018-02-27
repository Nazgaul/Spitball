using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;

namespace Cloudents.Infrastructure.Write
{
    public class SynonymWrite //:  ISynonymWrite
    {
        private readonly SearchServiceClient _client;

        public SynonymWrite(SearchServiceClient client)
        {
            _client = client;
        }


        public Task CreateOrUpdateAsync(string name, string synonyms, CancellationToken token)
        {
            var synonym = new SynonymMap(name, SynonymMapFormat.Solr, synonyms);
            return _client.SynonymMaps.CreateOrUpdateAsync(synonym, cancellationToken: token);
        }

        public void CreateEmpty(string name)
        {
            try
            {
                var synonym = new SynonymMap(name, SynonymMapFormat.Solr, string.Empty);
                _client.SynonymMaps.Create(synonym);
            }
#pragma warning disable CC0004 // Catch block cannot be empty
            catch (CloudException)
            {

            }
#pragma warning restore CC0004 // Catch block cannot be empty
        }
    }
}