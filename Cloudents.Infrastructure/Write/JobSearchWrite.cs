using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Extension;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write
{
    public class JobSearchWrite : SearchServiceWrite<Job>
    {
        public const string IndexName = "jobs2";
        public JobSearchWrite(SearchServiceClient client)
            : base(client, IndexName)
        {

        }

        public override Index GetIndexStructure(string indexName)
        {
            return new Index
            {
                Name = IndexName,
                Fields = new List<Field>
                {
                    new Field(nameof(Job.Id).CamelCase(), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(Job.Title).CamelCase(), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Job.Description).CamelCase(), DataType.String)
                    ,
                    new Field(nameof(Job.Company).CamelCase(), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Job.City).CamelCase(), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Job.State).CamelCase(), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Job.Compensation).CamelCase(), DataType.String)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Job.DateTime).CamelCase(), DataType.DateTimeOffset)
                    {
                        IsSortable = true
                    },
                    new Field(nameof(Job.Location).CamelCase(), DataType.GeographyPoint)
                    {
                        IsFilterable = true,
                        IsSortable = true
                    },
                    new Field(nameof(Job.Source).CamelCase(), DataType.String)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Job.Extra).CamelCase(), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Job.Url).CamelCase(), DataType.String)
                    ,
                    new Field(nameof(Job.InsertDate).CamelCase(), DataType.DateTimeOffset)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Job.JobType).CamelCase(), DataType.String)
                    {
                        IsFilterable = true,
                        IsFacetable = true
                    }
              }
            };
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
