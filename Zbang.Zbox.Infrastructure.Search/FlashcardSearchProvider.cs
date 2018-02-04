using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class FlashcardSearchProvider : IFlashcardWriteSearchProvider
    {
        private readonly string _indexName = "flashcard";
        private readonly ISearchConnection _connection;
        private bool _checkIndexExists;
        private readonly ISearchIndexClient _indexClient;
        private readonly ILogger _logger;

        public FlashcardSearchProvider( ISearchConnection connection, ILogger logger)
        {
            _connection = connection;
            _logger = logger;
            if (_connection.IsDevelop)
            {
                _indexName = _indexName + "-dev";
            }
            _indexClient = connection.SearchClient.Indexes.GetClient(_indexName);
        }

        private const string IdField = "id";
        private const string FrontCardsField = "front";
        private const string BackCardsField = "back";
        private const string ContentField = "metaContent";
        private const string NameField = "name";
        private const string UniversityNameField = "universityName";
        private const string BoxIdField = "boxId";
        private const string BoxNameField = "boxName";
        private const string UniversityIdField = "universityId";
        private const string UserIdsField = "userId";
        private const string ScoringProfileName = "university";

        private Index GetIndexStructure()
        {
            var index = new Index(_indexName, new[]
            {
                new Field(IdField, DataType.String) { IsKey = true, IsRetrievable = true},
                new Field(NameField, DataType.String) { IsSearchable = true, IsRetrievable = true},
                new Field(BoxNameField, DataType.String) { IsRetrievable = true},

                new Field(FrontCardsField, DataType.Collection(DataType.String)) { IsSearchable = true, IsRetrievable = true},
                new Field(BackCardsField, DataType.Collection(DataType.String)) { IsSearchable = true, IsRetrievable = true},

                new Field(UniversityNameField, DataType.String) { IsRetrievable = true},
                new Field(UniversityIdField, DataType.String) { IsRetrievable = true, IsFilterable = true},
                new Field(UserIdsField, DataType.Collection(DataType.String)) { IsFilterable = true, IsRetrievable = true} ,
                new Field(ContentField, DataType.String) { IsRetrievable = true},
                new Field(BoxIdField, DataType.Int64) { IsRetrievable = true}

            });
            var scoringFunction = new TagScoringFunction(UniversityIdField, 2, ScoringProfileName);
            var scoringProfile = new ScoringProfile("universityTag")
            {
                FunctionAggregation = ScoringFunctionAggregation.Sum,
                Functions = new List<ScoringFunction>
                {
                    scoringFunction
                }
            };
            index.ScoringProfiles = new List<ScoringProfile> { scoringProfile };
            return index;
        }

        private async Task BuildIndexAsync()
        {
            try
            {
                await _connection.SearchClient.Indexes.CreateOrUpdateAsync(GetIndexStructure()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Exception(ex);
            }
            _checkIndexExists = true;
        }

        public async Task<bool> UpdateDataAsync(IEnumerable<FlashcardSearchDto> flashcardToUpload, IEnumerable<long> flashcardToDelete, CancellationToken token)
        {
            if (!_checkIndexExists)
            {
                await BuildIndexAsync().ConfigureAwait(false);
            }
            var t1 = Task.CompletedTask;
            var t2 = Task.CompletedTask;
            if (flashcardToUpload != null)
            {
                var uploadBatch = flashcardToUpload.Select(s => new FlashcardSearch
                {
                    Id = s.Id.ToString(CultureInfo.InvariantCulture),
                    Name = s.Name,
                    BoxName = s.Course.Name,
                    Front = s.FrontCards.ToArray(),
                    MetaContent = string.Join(" ", s.FrontCards) + " " + string.Join(" ", s.BackCards).RemoveEndOfString(SearchConnection.DescriptionLength),
                    Back = s.BackCards.ToArray(),
                    UserId = s.UserIds.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray(),
                    UniversityName = s.University?.Name,
                    UniversityId = s.University?.Id.ToString() ?? "-1", //.HasValue ? s.UniversityId.ToString() : "-1",
                    BoxId = s.Course.Id,
                });
                var batch = IndexBatch.Upload(uploadBatch);
                if (batch.Actions.Any())
                {
                    t1 = _indexClient.Documents.IndexAsync(batch, cancellationToken: token);
                }
            }
            if (flashcardToDelete != null)
            {
                var deleteBatch = flashcardToDelete.Select(s =>
                     new FlashcardSearch
                     {
                         Id = s.ToString(CultureInfo.InvariantCulture)
                     });
                var batch = IndexBatch.Delete(deleteBatch);
                if (batch.Actions.Any())
                    t2 = _indexClient.Documents.IndexAsync(batch, cancellationToken: token);
            }
            await Task.WhenAll(t1, t2).ConfigureAwait(false);
            return true;
        }
    }
}
