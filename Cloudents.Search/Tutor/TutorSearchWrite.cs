using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace Cloudents.Search.Tutor
{
    public class TutorSearchWrite : SearchServiceWrite<Entities.Tutor>
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
                    new Field(nameof(Entities.Tutor.Id), DataType.String)
                    {
                        IsKey = true
                    },
                    new Field(nameof(Entities.Tutor.Name), DataType.String)
                    ,
                    new Field(nameof(Entities.Tutor.Image), DataType.String)
                    ,
                    new Field(nameof(Entities.Tutor.Url), DataType.String)
                    ,
                    new Field(nameof(Entities.Tutor.Description), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Entities.Tutor.City), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Entities.Tutor.State), DataType.String)
                    {
                        IsSearchable = true
                    },
                    new Field(nameof(Entities.Tutor.Location), DataType.GeographyPoint)
                    {
                        IsFilterable = true,
                        IsSortable = true
                    },
                    new Field(nameof(Entities.Tutor.Fee), DataType.Double)
                    {
                        IsSortable = true,
                        IsFilterable = true
                    },
                    new Field(nameof(Entities.Tutor.TutorFilter), DataType.Int32)
                    {
                        IsFilterable = true
                    },

                    new Field(nameof(Entities.Tutor.Source), DataType.String)
                    {
                        IsFilterable = true
                    },
                    new Field(nameof(Entities.Tutor.Extra), DataType.Collection(DataType.String))
                    {
                        IsSearchable = true
                    },

                    new Field(nameof(Entities.Tutor.InsertDate), DataType.DateTimeOffset)
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
                Filter = $"{nameof(Entities.Tutor.InsertDate)} lt {timeToDelete.ToUniversalTime():yyyy-MM-dd'T'hh:mm:ss'Z'} and {nameof(Entities.Tutor.Source)} eq '{source}'",
                Select = new[] { nameof(Entities.Tutor.Id) },
                Top = top
            };
            IList<SearchResult<Entities.Tutor>> result;
            do
            {
                var searchRetVal = await IndexClient.Documents.SearchAsync<Entities.Tutor>("*", parameters, cancellationToken: token)
                    ;
                result = searchRetVal.Results;

                await DeleteDataAsync(result.Select(s => s.Document.Id), token);

            } while (result.Count == top);
        }
    }
}
