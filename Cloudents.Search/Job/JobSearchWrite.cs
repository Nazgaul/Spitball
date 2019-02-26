using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Search.Job
{
    public class JobSearchWrite : SearchServiceWrite<Entities.Job>
    {
        public const string IndexName = "jobs3";

        public JobSearchWrite(SearchService client, ILogger logger)
            : base(client, IndexName,logger)
        {

        }

        protected override Index GetIndexStructure(string indexName)
        {
            return new Index
            {
                Name = IndexName,
                Fields = new List<Field>
                {
                    new Field(nameof(Entities.Job.Id), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(Entities.Job.Title), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Entities.Job.Description), DataType.String)
                    ,
                    new Field(nameof(Entities.Job.Company), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Entities.Job.City), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Entities.Job.State), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Entities.Job.Compensation), DataType.String)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Entities.Job.DateTime), DataType.DateTimeOffset)
                    {
                        IsSortable = true,
                        IsFilterable = true
                    },
                    new Field(nameof(Entities.Job.Location), DataType.GeographyPoint)
                    {
                        IsFilterable = true,
                        IsSortable = true
                    },
                    new Field(nameof(Entities.Job.Source), DataType.String)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Entities.Job.Extra), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Entities.Job.Url), DataType.String)
                    ,
                    new Field(nameof(Entities.Job.InsertDate), DataType.DateTimeOffset)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Entities.Job.JobType), DataType.String)
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
                Filter = $"{nameof(Entities.Job.InsertDate)} lt {timeToDelete.ToUniversalTime():yyyy-MM-dd'T'hh:mm:ss'Z'} and {nameof(Entities.Job.Source)} eq '{source}'",
                Select = new[] { nameof(Entities.Job.Id) },
                Top = top
            };
            IList<SearchResult<Entities.Job>> result;
            do
            {
                var searchRetVal = await IndexClient.Documents.SearchAsync<Entities.Job>("*", parameters, cancellationToken: token)
                    ;
                result = searchRetVal.Results;

                await DeleteDataAsync(result.Select(s => s.Document.Id), token);

            } while (result.Count == top);
        }
    }
}
