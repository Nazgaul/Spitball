using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Search;
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
                    new Field(nameof(Job.Id), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(Job.Title), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Job.Description), DataType.String)
                    ,
                    new Field(nameof(Job.Company), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Job.City), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Job.State), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Job.Compensation), DataType.String)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Job.DateTime), DataType.DateTimeOffset)
                    {
                        IsSortable = true
                    },
                    new Field(nameof(Job.Location), DataType.GeographyPoint)
                    {
                        IsFilterable = true,
                        IsSortable = true
                    },
                    new Field(nameof(Job.Source), DataType.String)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Job.Extra), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Job.Url), DataType.String)
                    ,
                    new Field(nameof(Job.InsertDate), DataType.DateTimeOffset)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Job.JobType), DataType.String)
                    {
                        IsFilterable = true,
                        IsFacetable = true
                    }
              }
            };
        }

        public async Task DeleteOldJobsAsync(string source, CancellationToken token)
        {
            const int top = 1000;
            var parameters = new SearchParameters
            {
                Filter = $"{nameof(Job.InsertDate)} lt {DateTime.UtcNow.AddDays(-4):yyyy-MM-dd'T'hh:mm:ss'Z'} and {nameof(Job.Source)} eq '{source}'",
                Select = new[] { nameof(Job.Id) },
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
