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
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class FlashcardSearchProvider : IFlashcardWriteSearchProvider, IFlashcardReadSearchProvider
    {
        private readonly string m_IndexName = "flashcard";
        private readonly ISearchFilterProvider m_FilterProvider;
        private readonly ISearchConnection m_Connection;
        private bool m_CheckIndexExists;
        private readonly ISearchIndexClient m_IndexClient;

        public FlashcardSearchProvider(ISearchFilterProvider filterProvider, ISearchConnection connection)
        {
            m_FilterProvider = filterProvider;
            m_Connection = connection;
            if (m_Connection.IsDevelop)
            {
                m_IndexName = m_IndexName + "-dev";
            }
            m_IndexClient = connection.SearchClient.Indexes.GetClient(m_IndexName);
        }

        private const string IdField = "id";
        private const string FrontCardsField = "front";
        private const string BackCardsField = "back";
        private const string ContentField = "metaContent";
        private const string NameField = "name";
        private const string UniversityNameField = "universityName";
        private const string BoxIdField = "boxId";
        private const string BoxNameField = "boxName";
        private const string UniversityidField = "universityId";
        private const string UseridsField = "userId";
        private const string ScoringProfileName = "university";

        private Index GetIndexStructure()
        {
            var index = new Index(m_IndexName, new[]
            {
                new Field(IdField, DataType.String) { IsKey = true, IsRetrievable = true},
                new Field(NameField, DataType.String) { IsSearchable = true, IsRetrievable = true},
                new Field(BoxNameField, DataType.String) { IsRetrievable = true},

                new Field(FrontCardsField, DataType.Collection(DataType.String)) { IsSearchable = true, IsRetrievable = true},
                new Field(BackCardsField, DataType.Collection(DataType.String)) { IsSearchable = true, IsRetrievable = true},


                new Field(UniversityNameField, DataType.String) { IsRetrievable = true},
                new Field(UniversityidField, DataType.String) { IsRetrievable = true, IsFilterable = true},
                new Field(UseridsField, DataType.Collection(DataType.String)) { IsFilterable = true, IsRetrievable = true} ,
                new Field(ContentField, DataType.String) { IsRetrievable = true},
                new Field(BoxIdField, DataType.Int64) { IsRetrievable = true}

            });
            var scoringFunction = new TagScoringFunction(UniversityidField, 2, ScoringProfileName);
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
                await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetIndexStructure());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on box build index", ex);
            }
            m_CheckIndexExists = true;
        }


        public async Task<bool> UpdateDataAsync(IEnumerable<FlashcardSearchDto> flashcardToUpload, IEnumerable<long> flashcardToDelete, CancellationToken token)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndexAsync();
            }
            var t1 = Task.CompletedTask;
            var t2 = Task.CompletedTask;
            if (flashcardToUpload != null)
            {

                var uploadBatch = flashcardToUpload.Select(s => new FlashcardSearch
                {
                    Id = s.Id.ToString(CultureInfo.InvariantCulture),
                    Name = s.Name,
                    BoxName = s.BoxName,
                    Front = s.FrontCards.ToArray(),
                    MetaContent = string.Join(" ", s.FrontCards) + " " + string.Join(" ", s.BackCards).RemoveEndOfString(SeachConnection.DescriptionLength),
                    Back = s.BackCards.ToArray(),
                    UserId = s.UserIds.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray(),
                    UniversityName = s.UniversityName,
                    UniversityId = s.UniversityId.HasValue ? s.UniversityId.ToString() : "-1",
                    BoxId = s.BoxId,
                });
                var batch = IndexBatch.Upload(uploadBatch);
                if (batch.Actions.Any())
                {
                    t1 = m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
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
                    t2 = m_IndexClient.Documents.IndexAsync(batch, cancellationToken: token);
            }
            await Task.WhenAll(t1, t2);
            return true;

        }

        public async Task<IEnumerable<SearchFlashcard>> SearchFlashcardAsync(SearchQuery query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            var filter = await m_FilterProvider.BuildFilterExpressionAsync(
              query.UniversityId, UniversityidField, UseridsField, query.UserId);

            //if we put asterisk highlight is not working
            var result = await m_IndexClient.Documents.SearchAsync<FlashcardSearch>(query.Term, new SearchParameters
            {
                Filter = filter,
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                ScoringProfile = "universityTag",
                ScoringParameters = new[] { new ScoringParameter("university", new[] { query.UniversityId.ToString() }) },
                HighlightFields = new[] { FrontCardsField, BackCardsField, NameField },
                Select = new[] { NameField, IdField, BoxNameField, UniversityNameField, BoxIdField, ContentField }
            }, cancellationToken: cancelToken);

            return result.Results.Select(s => new SearchFlashcard
            {
                BoxName = s.Document.BoxName,
                Content = HighLightInField(s, new[] { FrontCardsField, BackCardsField }, s.Document.MetaContent),
                Id = long.Parse(s.Document.Id),
                Name = HighLightInField(s, new[] { NameField }, s.Document.Name),
                UniName = s.Document.UniversityName,
                BoxId = s.Document.BoxId.GetValueOrDefault()
            });

        }
        private static string HighLightInField(SearchResult<FlashcardSearch> record, IEnumerable<string> fields, string defaultValue)
        {
            if (record.Highlights == null)
            {
                return defaultValue;
            }
            foreach (var field in fields)
            {
                IList<string> highLight;
                if (record.Highlights.TryGetValue(field, out highLight))
                {
                    return string.Join("...", highLight);
                }
            }
            return defaultValue;
        }


      
    }
}
