using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Infrastructure.Write.Tutor
{
    public class TutorSearchWrite : SearchServiceWrite<Core.Entities.Search.Tutor>
    {
        public const string IndexName = "tutors4";
        public TutorSearchWrite(SearchServiceClient client)
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
                    new Field(nameof(Core.Entities.Search.Tutor.Id), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(Core.Entities.Search.Tutor.Name), DataType.String)
                    ,
                    new Field(nameof(Core.Entities.Search.Tutor.Image), DataType.String)
                    ,
                    new Field(nameof(Core.Entities.Search.Tutor.Url), DataType.String)
                    ,
                    new Field(nameof(Core.Entities.Search.Tutor.Description), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Core.Entities.Search.Tutor.City), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Core.Entities.Search.Tutor.State), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Core.Entities.Search.Tutor.Location), DataType.GeographyPoint)
                    {
                        IsFilterable = true,
                        IsSortable = true
                    },
                    new Field(nameof(Core.Entities.Search.Tutor.Fee), DataType.Double)
                    {
                        IsSortable = true,
                        IsFilterable = true
                    },
                    new Field(nameof(Core.Entities.Search.Tutor.TutorFilter), DataType.Int32)
                    {
                        IsFilterable = true
                    },

                    new Field(nameof(Core.Entities.Search.Tutor.Source), DataType.String)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Core.Entities.Search.Tutor.Extra), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true
                    },

                    new Field(nameof(Core.Entities.Search.Tutor.InsertDate), DataType.DateTimeOffset)
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
                Filter = $"{nameof(Core.Entities.Search.Tutor.InsertDate)} lt {timeToDelete.ToUniversalTime():yyyy-MM-dd'T'hh:mm:ss'Z'} and {nameof(Core.Entities.Search.Tutor.Source)} eq '{source}'",
                Select = new[] { nameof(Core.Entities.Search.Tutor.Id) },
                Top = top
            };
            IList<SearchResult<Core.Entities.Search.Tutor>> result;
            do
            {
                var searchRetVal = await IndexClient.Documents.SearchAsync<Core.Entities.Search.Tutor>("*", parameters, cancellationToken: token)
                    .ConfigureAwait(false);
                result = searchRetVal.Results;

                await DeleteDataAsync(result.Select(s => s.Document.Id), token).ConfigureAwait(false);

            } while (result.Count == top);
        }
    }
}
