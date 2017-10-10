using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class JobsProvider : SearchServiceWrite<Job>
    {
        public JobsProvider(ISearchConnection connection, ILogger logger)
            : base(connection, "jobs", logger)
        {

        }

        public override Index GetIndexStructure(string indexName)
        {
            var definition = new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Job>()
            };
            return definition;
        }

        public async Task DeleteOldJobsAsync(CancellationToken token)
        {
            const int top = 1000;
            var parameters = new SearchParameters
            {
                Filter = $"insertDate lt {DateTime.UtcNow.AddDays(-4):yyyy-MM-dd'T'hh:mm:ss'Z'}",
                Select = new[] { "id" },
                Top = top
            };
            IList<SearchResult<Job>> result;
            do
            {
                var searchRetVal = await IndexClient.Documents.SearchAsync<Job>("*", parameters, cancellationToken: token)
                    .ConfigureAwait(false);
                result = searchRetVal.Results;

                await DeleteDataAsync(result.Select(s => s.Document.Id), token).ConfigureAwait(false);

            } while (result.Count == top);
        }
    }

   
}
