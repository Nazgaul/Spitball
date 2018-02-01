using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write.Job
{
    public class JobSearchWrite : SearchServiceWrite<Core.Entities.Search.Job>
    {
        public const string IndexName = "jobs3";
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
                    new Field(nameof(Core.Entities.Search.Job.Id), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(Core.Entities.Search.Job.Title), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Core.Entities.Search.Job.Description), DataType.String)
                    ,
                    new Field(nameof(Core.Entities.Search.Job.Company), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Core.Entities.Search.Job.City), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Core.Entities.Search.Job.State), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Core.Entities.Search.Job.Compensation), DataType.String)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Core.Entities.Search.Job.DateTime), DataType.DateTimeOffset)
                    {
                        IsSortable = true,
                        IsFilterable = true
                    },
                    new Field(nameof(Core.Entities.Search.Job.Location), DataType.GeographyPoint)
                    {
                        IsFilterable = true,
                        IsSortable = true
                    },
                    new Field(nameof(Core.Entities.Search.Job.Source), DataType.String)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Core.Entities.Search.Job.Extra), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Core.Entities.Search.Job.Url), DataType.String)
                    ,
                    new Field(nameof(Core.Entities.Search.Job.InsertDate), DataType.DateTimeOffset)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Core.Entities.Search.Job.JobType), DataType.String)
                    {
                        IsFilterable = true,
                        IsFacetable = true
                    }
              }
            };
        }

        public async Task DeleteOldJobsAsync(string source, DateTime timeToDelete, CancellationToken token)
        {
            const int top = 1000;
            var parameters = new SearchParameters
            {
                Filter = $"{nameof(Core.Entities.Search.Job.InsertDate)} lt {timeToDelete.ToUniversalTime():yyyy-MM-dd'T'hh:mm:ss'Z'} and {nameof(Core.Entities.Search.Job.Source)} eq '{source}'",
                Select = new[] { nameof(Core.Entities.Search.Job.Id) },
                Top = top
            };
            IList<SearchResult<Core.Entities.Search.Job>> result;
            do
            {
                var searchRetVal = await IndexClient.Documents.SearchAsync<Core.Entities.Search.Job>("*", parameters, cancellationToken: token)
                    .ConfigureAwait(false);
                result = searchRetVal.Results;

                await DeleteDataAsync(result.Select(s => s.Document.Id), token).ConfigureAwait(false);

            } while (result.Count == top);
        }
    }
}
