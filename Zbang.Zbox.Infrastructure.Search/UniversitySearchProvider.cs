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
                m_IndexName = m_IndexName + "-dev";
            }
            m_IndexClient = connection.SearchClient.Indexes.GetClient(m_IndexName);
        }

        private const string IdField = "id";
        private const string NameField = "name";
        private const string NameField2 = "name2";
        private const string ExtraOneField = "extra1";
        private const string ExtraTwoField = "extra2";
        private const string ImageField = "imageField";

        private const string CountryField = "coutry";
        private const string MembersCountField = "membersCount";
        private const string MembersImagesField = "membersImages";

        private const string CountryScoringProfile = "countryTag";

        private const string NameSuggest = "nameSuggest";

        private Index GetUniversityIndex()
        {
            var index = new Index(m_IndexName, new[]
            {
                new Field(IdField, DataType.String) {IsKey = true, IsRetrievable = true},
                new Field(NameField, DataType.String) {IsRetrievable = true, IsSearchable = true},
                new Field(NameField2, DataType.String) {IsRetrievable = true, IsSearchable = true},
                new Field(ExtraOneField, DataType.String) {IsSearchable = true, IsRetrievable = false},
                new Field(ExtraTwoField, DataType.String) {IsSearchable = true, IsRetrievable = false},
                new Field(ImageField, DataType.String) {IsRetrievable = true},
                new Field(CountryField, DataType.String) {IsRetrievable = true, IsFilterable = true},
                new Field(MembersCountField, DataType.Int32) {IsRetrievable = true},
                new Field(MembersImagesField, DataType.Collection(DataType.String)) {IsRetrievable = true}

            });
            var scoringFunction = new TagScoringFunction("country", 3, CountryField);
            //var scoringFunction = new TagScoringFunction(new TagScoringParameters("country"),
            //   CountryField, 3);
            var scoringProfile = new ScoringProfile(CountryScoringProfile)
            {
                FunctionAggregation = ScoringFunctionAggregation.Sum,

            };
            scoringProfile.Functions.Add(scoringFunction);
            index.ScoringProfiles.Add(scoringProfile);

            var suggester = new Suggester(NameSuggest, SuggesterSearchMode.AnalyzingInfixMatching, NameField2);
            index.Suggesters.Add(suggester);
            return index;

        }


        private async Task BuildIndexAsync()
        {
            try
            {
                await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetUniversityIndex());
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
            //{
            //    return null;
            //}
            var listOfSelectParams = new[] { IdField, NameField2, ImageField, MembersCountField, MembersImagesField };
            var searchParametes = new SearchParameters
            {
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                Select = listOfSelectParams,
            };
            var term = query.Term;
            if (string.IsNullOrEmpty(term))//obsolete
            {
                searchParametes.ScoringProfile = CountryScoringProfile;
                searchParametes.ScoringParameters = new[] { new ScoringParameter("country", new[] { query.Country }) };
            }
            else
            {
                term = query.Term.Replace("\"", string.Empty);
            }

            var tResult = m_IndexClient.Documents.SearchAsync<UniversitySearch>(term + "*", searchParametes, cancellationToken: cancelToken);

            var tSuggest = CompletedTask;
            if (!string.IsNullOrEmpty(query.Term) && query.Term.Length >= 3 && query.PageNumber == 0)
            {
                tSuggest = m_IndexClient.Documents.SuggestAsync<UniversitySearch>(query.Term, NameSuggest, new SuggestParameters
                {
                    UseFuzzyMatching = true,
                    Select = listOfSelectParams
                }, cancellationToken: cancelToken);
            }
            await Task.WhenAll(tResult, tSuggest);


            var result = tResult.Result.Results.Select(
                    s => new UniversityByPrefixDto
                    {
                        Id = long.Parse(s.Document.Id),
                        Image = s.Document.Image,
                        Name = s.Document.Name2,
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
                          Name = s.Document.Name2,
                          NumOfUsers = s.Document.MembersCount ?? 0,
                          UserImages = s.Document.MembersImages
                      }));
            }

            return result;

        }
        private static readonly Task<DocumentSuggestResult<UniversitySearch>> CompletedTask = Task.FromResult<DocumentSuggestResult<UniversitySearch>>(null);

        public async Task<bool> UpdateDataAsync(IEnumerable<UniversitySearchDto> universityToUpload, IEnumerable<long> universityToDelete)
        {
            if (!m_CheckIndexExists)
            {
                // await BuildIndex();
            }

            //var listOfCommands = new List<IndexAction<UniversitySearch>>();
            if (universityToUpload != null)
            {
                //listOfCommands.AddRange(
                var uploadBatch = universityToUpload.Select(s =>
                        new UniversitySearch
                        {
                            Id = s.Id.ToString(CultureInfo.InvariantCulture),
                            Name = s.Name.Trim(),
                            Name2 = s.Name.Trim(),
                            Extra1 = s.Extra?.Replace(","," "),
                            Extra2 = string.Join(
                                " ",
                                s.Name.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                                    .Where(w => w.StartsWith("ה") || w.StartsWith("ל"))
                                    .Select(s1 => s1.Remove(0, 1))
                                    .Union(s.Name.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                                        .Where(w => w.Contains('"'))

                                        .Select(s2 => s2.Replace("\"", string.Empty)))),
                            Image = string.IsNullOrEmpty(s.Image) ? null : s.Image.Trim(),
                            Country = s.Country,
                            MembersCount = s.NoOfUsers,

                            MembersImages = s.UsersImages.Where(w => w != null).ToArray()
                        }
                            );

                //);

                var batch = IndexBatch.Upload(uploadBatch);
                if (batch.Actions.Any())
                    await m_IndexClient.Documents.IndexAsync(batch);


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
                    await m_IndexClient.Documents.IndexAsync(batch);
                

            }
            return true;
            
        }


    }
}
