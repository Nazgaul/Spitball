using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
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
        private readonly SearchIndexClient m_IndexClient;

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
        private const string ExtraOneField = "extra1";
        private const string ExtraTwoField = "extra2";
        private const string ImageField = "imageField";

        private const string CountryField = "coutry";
        private const string MembersCountField = "membersCount";
        private const string MembersImagesField = "membersImages";

        private const string CountryScoringProfile = "countryTag";

        // private const string NameSuggest = "nameSuggest";

        private Index GetUniversityIndex()
        {
            var index = new Index(m_IndexName, new[]
            {
                new Field(IdField, DataType.String) {IsKey = true, IsRetrievable = true},
                new Field(NameField, DataType.String) {IsRetrievable = true, IsSearchable = true},
                new Field(ExtraOneField, DataType.String) {IsSearchable = true, IsRetrievable = false},
                new Field(ExtraTwoField, DataType.String) {IsSearchable = true, IsRetrievable = false},
                new Field(ImageField, DataType.String) {IsRetrievable = true},
                new Field(CountryField, DataType.String) {IsRetrievable = true, IsFilterable = true},
                new Field(MembersCountField, DataType.Int32) {IsRetrievable = true},
                new Field(MembersImagesField, DataType.Collection(DataType.String)) {IsRetrievable = true}

            });
            var scoringFunction = new TagScoringFunction(new TagScoringParameters("country"),
               CountryField, 3);
            var scoringProfile = new ScoringProfile(CountryScoringProfile)
            {
                FunctionAggregation = ScoringFunctionAggregation.Sum,

            };
            scoringProfile.Functions.Add(scoringFunction);
            index.ScoringProfiles.Add(scoringProfile);
            //{
            //    Suggesters = new[]
            //    {
            //        new Suggester(NameSuggest, SuggesterSearchMode.AnalyzingInfixMatching, NameField)
            //    }
            //};
            return index;

        }

/*
        private async Task BuildIndex()
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
*/




        public async Task<IEnumerable<UniversityByPrefixDto>> SearchUniversityAsync(UniversitySearchQuery query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException("query");

            if (string.IsNullOrEmpty(query.Term) && string.IsNullOrEmpty(query.Country))
            {
                throw new ArgumentNullException("query");
            }
            //{
            //    return null;
            //}
            var searchParametes = new SearchParameters
            {
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                Select = new[] { IdField, NameField, ImageField, MembersCountField, MembersImagesField },
            };

            if (string.IsNullOrEmpty(query.Term))
            {
                searchParametes.ScoringProfile = CountryScoringProfile;
                searchParametes.ScoringParameters = new[] { "country:" + query.Country };
            }

            var tResult = m_IndexClient.Documents.SearchAsync<UniversitySearch>(query.Term + "*", searchParametes, cancelToken);

            //var tSuggest = Task.FromResult<Task<DocumentSuggestResponse<UniversitySearch>>>(null);
            //if (query.Term.Length >= 3 && query.PageNumber == 0)
            //{
            //    tSuggest = m_IndexClient.Documents.SuggestAsync<UniversitySearch>(query.Term, NameSuggest, new SuggestParameters
            //     {
            //         UseFuzzyMatching = true,
            //         Select = new[] { IdField, NameField, ImageField }
            //     });
            //}
            await Task.WhenAll(tResult);




            return
                tResult.Result.Select(
                    s => new UniversityByPrefixDto
                    {
                        Id = long.Parse(s.Document.Id),
                        Image = s.Document.Image,
                        Name = s.Document.Name,
                        NumOfUsers = s.Document.MembersCount.HasValue ? s.Document.MembersCount.Value : 0,
                        UserImages = s.Document.MembersImages
                    });

        }


        public async Task<bool> UpdateData(IEnumerable<UniversitySearchDto> universityToUpload, IEnumerable<long> universityToDelete)
        {
            if (!m_CheckIndexExists)
            {
                //await BuildIndex();
            }

            var listOfCommands = new List<IndexAction<UniversitySearch>>();
            if (universityToUpload != null)
            {
                listOfCommands.AddRange(
                    universityToUpload.Select(s => new IndexAction<UniversitySearch>(IndexActionType.MergeOrUpload, new UniversitySearch
                    {
                        Id = s.Id.ToString(CultureInfo.InvariantCulture),
                        Name = s.Name.Trim(),
                        Extra1 = s.Extra,
                        Extra2 = String.Join(
                               " ",
                               s.Name.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                                   .Where(w => w.StartsWith("ה") || w.StartsWith("ל"))
                                   .Select(s1 => s1.Remove(0, 1))),
                        Image = string.IsNullOrEmpty(s.Image) ? null : s.Image.Trim(),
                        Country = s.Country,
                        MembersCount = s.NoOfUsers,
                        MembersImages = s.UsersImages.Where(w => w != null).ToArray()
                    })));



            }
            if (universityToDelete != null)
            {
                listOfCommands.AddRange(universityToDelete.Select(s =>
                   new IndexAction<UniversitySearch>(IndexActionType.Delete, new UniversitySearch
                   {
                       Id = s.ToString(CultureInfo.InvariantCulture)
                   })));

            }
            var commands = listOfCommands.ToArray();
            if (commands.Length <= 0) return true;
            try
            {
                await m_IndexClient.Documents.IndexAsync(IndexBatch.Create(listOfCommands.ToArray()));
            }
            catch (IndexBatchException ex)
            {
                TraceLog.WriteError("Failed to index some of the documents: " +
                                    String.Join(", ",
                                        ex.IndexResponse.Results.Where(r => !r.Succeeded).Select(r => r.Key)));
                return false;
            }

            return true;
        }


    }
}
