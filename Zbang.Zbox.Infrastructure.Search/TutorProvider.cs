using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Search
{
   public  class TutorProvider : SearchServiceWrite<Tutor>
    {
        public TutorProvider(ISearchConnection connection, ILogger logger) : base(connection,"tutors", logger)
        {
            
        }

        public override Index GetIndexStructure(string indexName)
        {
            var definition = new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Tutor>()
            };
            return definition;
        }

        public async Task<IEnumerable<string>> GetOldTutorsAsync(CancellationToken token)
        {
            var parameters = new SearchParameters()
            {
                Filter = $"insertDate lt {DateTime.UtcNow.AddDays(-2):yyyy-MM-dd'T'hh:mm:ss'Z'}",
                Select = new[] { "id" }
            };
            var result = await IndexClient.Documents.SearchAsync<Job>("*", parameters, cancellationToken: token).ConfigureAwait(false);
            return result.Results.Select(s => s.Document.Id);
        }
    }
}
