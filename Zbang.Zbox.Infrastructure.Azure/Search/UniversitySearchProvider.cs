using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RedDog.Search.Http;
using RedDog.Search.Model;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class UniversitySearchProvider : IUniversityReadSearchProvider, IUniversityWriteSearchProvider2
    {
        private const string IndexName = "universities2";
        

        private async Task BuildIndex()
        {
            var response = await SeachConnection.Instance.IndexManagement.GetIndexAsync(IndexName);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await SeachConnection.Instance.IndexManagement.CreateIndexAsync(GetUniversityIndex());
            }

        }


       

        public async Task<IEnumerable<UniversityByPrefixDto>> SearchUniversity(UniversitySearchQuery query)
        {

            if (string.IsNullOrEmpty(query.Term))
            {
                return null;
            }

            var searchTask = SeachConnection.Instance.IndexQuery.SearchAsync(IndexName,
                  new SearchQuery(query.Term + "*")
                  {
                      Select = "id,name,imageField",
                      Top = query.RowsPerPage,
                      Skip = query.RowsPerPage * query.PageNumber
                  });
            var suggestTask = Task.FromResult<IApiResponse<SuggestionResult>>(null);
            if (query.Term.Length >= 3 && query.PageNumber == 0)
            {
                suggestTask = SeachConnection.Instance.IndexQuery.SuggestAsync(IndexName,
                    new SuggestionQuery(query.Term)
                    {
                        Fuzzy = true,
                        Select = "id,name,imageField"
                    });
            }
            await Task.WhenAll(searchTask, suggestTask);
            if (searchTask.Result.Body.Records.Any())
            {
                return searchTask.Result.Body.Records.Select(s => new UniversityByPrefixDto(
                 s.Properties["name"].ToString(),
                 s.Properties["imageField"].ToString(),
                 Convert.ToInt64(s.Properties["id"])
                 ));
            }
            if (suggestTask.Result != null)
            {
                return suggestTask.Result.Body.Records.Select(s => new UniversityByPrefixDto(
                    s.Properties["name"].ToString(),
                    s.Properties["imageField"].ToString(),
                    Convert.ToInt64(s.Properties["id"])
                    ));
            }
            return null;

        }


        public async Task<bool> UpdateData(IEnumerable<UniversitySearchDto> universityToUpload, IEnumerable<long> universityToDelete)
        {
            await BuildIndex();

            var listOfCommands = new List<IndexOperation>();
            if (universityToUpload != null)
            {
                listOfCommands.AddRange(universityToUpload.Select(s =>
                {
                    var x = new IndexOperation(IndexOperationType.Upload, "id",
                        s.Id.ToString(CultureInfo.InvariantCulture))
                        .WithProperty("name", s.Name)
                        .WithProperty("extra1", s.Extra)
                        .WithProperty("extra2", String.Join(
                            " ",
                            s.Name.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(w => w.StartsWith("ה") || w.StartsWith("ל"))
                                .Select(s1 => s1.Remove(0, 1))))
                        .WithProperty("imageField", s.Image);
                    return x;
                }));
            }
            if (universityToDelete != null)
            {
                listOfCommands.AddRange(universityToDelete.Select(s =>
                    new IndexOperation(IndexOperationType.Delete, "id", s.ToString(CultureInfo.InvariantCulture))
                    ));
            }
            var commands = listOfCommands.ToArray();
            if (commands.Length > 0)
            {

                var retVal = await SeachConnection.Instance.IndexManagement.PopulateAsync(IndexName, listOfCommands.ToArray());
                if (!retVal.IsSuccess)
                {
                    TraceLog.WriteError("On update search" + retVal.Error.Message);
                }
                return retVal.IsSuccess;
            }
            return true;
        }

        private Index GetUniversityIndex()
        {
            return new Index(IndexName)
                   .WithStringField("id", f => f
                       .IsKey()
                       .IsRetrievable()
                       )
                   .WithStringField("name", f => f
                       .IsRetrievable()
                       .IsSearchable()
                       .IsFilterable(false)
                       .SupportSuggestions())
                   .WithStringField("extra1", f => f
                       .IsSearchable()
                       .IsFilterable(false)
                       )
                   .WithStringField("extra2", f => f
                       .IsFilterable(false)
                       .IsSearchable()
                       )
                   .WithStringField("imageField", f => f
                       .IsRetrievable()
                       );
        }
    }
}
