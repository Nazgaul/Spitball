using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class UniversitySearchProvider : IUniversityReadSearchProvider, IUniversityWriteSearchProvider2
    {
        private readonly string m_IndexName = "universities2";
        private bool m_CheckIndexExists;
        private readonly ISearchConnection m_Connection;
        private readonly ISearchIndexClient m_IndexClient;

        public UniversitySearchProvider(ISearchConnection connection)
        {
            m_Connection = connection;
            if (m_Connection.IsDevelop)
            {
                m_IndexName += "-dev";
            }
            m_IndexClient = connection.SearchClient.Indexes.GetClient(m_IndexName);
        }

        private const string IdField = "id";
        private const string NameField3 = "name3";
        private const string ImageField = "imageField";

        internal const string CountryField = "coutry";
        private const string MembersCountField = "membersCount";
        private const string MembersImagesField = "membersImages";

        private const string CountryScoringProfile = "countryTag";

        private Index GetUniversityIndex()
        {
            var scoringFunction = new TagScoringFunction(CountryField, 3, new TagScoringParameters("country"));
            var scoringProfile = new ScoringProfile(CountryScoringProfile,
                functions: new ScoringFunction[] { scoringFunction }, functionAggregation: ScoringFunctionAggregation.Sum);

            var suggester = new Suggester("sg",  NameField3, nameof(UniversitySearch.Extra3).ToLowerInvariant());

            var index = new Index
            {
                Name = m_IndexName,
                Fields = FieldBuilder.BuildForType<UniversitySearch>(),
                ScoringProfiles = new[] { scoringProfile },
                Suggesters = new[] { suggester }
            };
            return index;
        }

        private async Task BuildIndexAsync()
        {
            try
            {
                await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetUniversityIndex()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on box build index", ex);
            }
            m_CheckIndexExists = true;
        }

        public async Task<IEnumerable<UniversityByPrefixDto>> SearchUniversityAsync(UniversitySearchQuery query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            if (string.IsNullOrEmpty(query.Term) && string.IsNullOrEmpty(query.Country))
            {
                throw new ArgumentNullException(nameof(query));
            }
            var listOfSelectParams = new[] { IdField, NameField3, ImageField, MembersCountField, MembersImagesField };
            var searchParameter = new SearchParameters
            {
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                Select = listOfSelectParams,
            };
            var term = query.Term;
            if (string.IsNullOrEmpty(term))//obsolete
            {
                searchParameter.ScoringProfile = CountryScoringProfile;
                searchParameter.ScoringParameters = new[] { new ScoringParameter("country", new[] { query.Country }) };
            }
            else
            {
                term = query.Term.Replace("\"", string.Empty);
            }

            var tResult = m_IndexClient.Documents.SearchAsync<UniversitySearch>(term + "*", searchParameter, cancellationToken: cancelToken);

            var tSuggest = CompletedTask;
            if (!string.IsNullOrEmpty(query.Term) && query.Term.Length >= 3 && query.PageNumber == 0)
            {
                tSuggest = m_IndexClient.Documents.SuggestAsync<UniversitySearch>(query.Term, "sg", new SuggestParameters
                {
                    UseFuzzyMatching = true,
                    Select = listOfSelectParams
                }, cancellationToken: cancelToken);
            }
            await Task.WhenAll(tResult, tSuggest).ConfigureAwait(false);

            var result = tResult.Result.Results.Select(
                    s => new UniversityByPrefixDto
                    {
                        Id = long.Parse(s.Document.Id),
                        Image = s.Document.Image,
                        Name = s.Document.Name3,
                        NumOfUsers = s.Document.MembersCount ?? 0,
                        UserImages = s.Document.MembersImages
                    });
            if (tSuggest.Result != null)
            {
                result = result.Union(tSuggest.Result.Results.Select(
                      s => new UniversityByPrefixDto
                      {
                          Id = long.Parse(s.Document.Id),
                          Image = s.Document.Image,
                          Name = s.Document.Name3,
                          NumOfUsers = s.Document.MembersCount ?? 0,
                          UserImages = s.Document.MembersImages
                      }));
            }

            return result;
        }

        private static readonly Task<DocumentSuggestResult<UniversitySearch>> CompletedTask = Task.FromResult<DocumentSuggestResult<UniversitySearch>>(null);

        public async Task<bool> UpdateDataAsync(IEnumerable<UniversitySearchDto> universityToUpload, IEnumerable<long> universityToDelete)
        {
            //if (!m_CheckIndexExists)
            //{
            //    await BuildIndexAsync().ConfigureAwait(false);
            //}

            //var listOfCommands = new List<IndexAction<UniversitySearch>>();
            if (universityToUpload != null)
            {
                //listOfCommands.AddRange(
                var uploadBatch = universityToUpload.Select(s =>

                        new UniversitySearch
                        {
                            Id = s.Id.ToString(CultureInfo.InvariantCulture),
#pragma warning disable 618 // we need to populate this field
                            Name = s.Name.Trim(),
                            Name2 = s.Name.Trim(),
#pragma warning restore 618
                            Name3 = s.Name.Trim(),

                            Extra3 = s.Extra?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries),
                            Image = s.Image?.Trim(),
                            Country = s.Country.ToLowerInvariant(),
                            MembersCount = s.NoOfUsers,
                            GeographyPoint = s.Latitude.HasValue && s.Longitude.HasValue ? GeographyPoint.Create(s.Latitude.Value,s.Longitude.Value) : null,
                            MembersImages = s.UsersImages.Where(w => w != null).ToArray()

                        }
                            );

                //);

                var batch = IndexBatch.Upload(uploadBatch);
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch).ConfigureAwait(false);
            }
            if (universityToDelete != null)
            {
                var deleteBatch = universityToDelete.Select(s =>
                new UniversitySearch
                {
                    Id = s.ToString(CultureInfo.InvariantCulture)
                });
                var batch = IndexBatch.Delete(deleteBatch);
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch).ConfigureAwait(false);
            }
            return true;
        }
    }
}
