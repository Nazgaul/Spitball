using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.Interfaces;
using Cloudents.Infrastructure.Search;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write.Tutor
{
    public class TutorSearchWrite : SearchServiceWrite<Application.Entities.Search.Tutor>
    {
        public const string IndexName = "tutors4";

        public TutorSearchWrite(SearchService client, ILogger logger)
            : base(client, IndexName,logger)
        {

        }

        protected override Index GetIndexStructure(string indexName)
        {
            return new Index
            {
                Name = indexName,
                Fields = new List<Field>
                {
                    new Field(nameof(Application.Entities.Search.Tutor.Id), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(Application.Entities.Search.Tutor.Name), DataType.String)
                    ,
                    new Field(nameof(Application.Entities.Search.Tutor.Image), DataType.String)
                    ,
                    new Field(nameof(Application.Entities.Search.Tutor.Url), DataType.String)
                    ,
                    new Field(nameof(Application.Entities.Search.Tutor.Description), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Application.Entities.Search.Tutor.City), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Application.Entities.Search.Tutor.State), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Application.Entities.Search.Tutor.Location), DataType.GeographyPoint)
                    {
                        IsFilterable = true,
                        IsSortable = true
                    },
                    new Field(nameof(Application.Entities.Search.Tutor.Fee), DataType.Double)
                    {
                        IsSortable = true,
                        IsFilterable = true
                    },
                    new Field(nameof(Application.Entities.Search.Tutor.TutorFilter), DataType.Int32)
                    {
                        IsFilterable = true
                    },

                    new Field(nameof(Application.Entities.Search.Tutor.Source), DataType.String)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Application.Entities.Search.Tutor.Extra), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true
                    },

                    new Field(nameof(Application.Entities.Search.Tutor.InsertDate), DataType.DateTimeOffset)
                    {
                        IsFilterable = true
                    }
              }
            };
        }

        public async Task DeleteOldTutorsAsync(string source, DateTime timeToDelete, CancellationToken token)
        {
            const int top = 1000;
            var parameters = new SearchParameters
            {
                Filter = $"{nameof(Application.Entities.Search.Tutor.InsertDate)} lt {timeToDelete.ToUniversalTime():yyyy-MM-dd'T'hh:mm:ss'Z'} and {nameof(Application.Entities.Search.Tutor.Source)} eq '{source}'",
                Select = new[] { nameof(Application.Entities.Search.Tutor.Id) },
                Top = top
            };
            IList<SearchResult<Application.Entities.Search.Tutor>> result;
            do
            {
                var searchRetVal = await IndexClient.Documents.SearchAsync<Application.Entities.Search.Tutor>("*", parameters, cancellationToken: token)
                    .ConfigureAwait(false);
                result = searchRetVal.Results;

                await DeleteDataAsync(result.Select(s => s.Document.Id), token).ConfigureAwait(false);

            } while (result.Count == top);
        }
    }
}
