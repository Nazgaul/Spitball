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
    public class TutorProvider : SearchServiceWrite<Tutor>
    {
        public TutorProvider(ISearchConnection connection, ILogger logger) : base(connection, "tutors", logger)
        {

        }

        public override Index GetIndexStructure(string indexName)
        {
            var definition = new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<Tutor>()
            };
            definition.Fields.First(f => f.Name == nameof(Tutor.Subjects).ToLowerInvariant()).SynonymMaps =
                new[] { SearchConnection.TutorSynonymMap };
            return definition;
        }

        public async Task DeleteOldTutorsAsync(CancellationToken token)
        {
            const int top = 1000;
            var parameters = new SearchParameters
            {
                Filter = $"insertDate lt {DateTime.UtcNow.AddDays(-2):yyyy-MM-dd'T'hh:mm:ss'Z'}",
                Select = new[] { "id" },
                Top = top
            };
            IList<SearchResult<Tutor>> result;
            do
            {
                var searchRetVal = await IndexClient.Documents.SearchAsync<Tutor>("*", parameters, cancellationToken: token)
                    .ConfigureAwait(false);
                result = searchRetVal.Results;

                await DeleteDataAsync(result.Select(s => s.Document.Id), token).ConfigureAwait(false);

            } while (result.Count == top);
        }
    }
}
