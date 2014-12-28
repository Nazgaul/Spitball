using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedDog.Search;
using RedDog.Search.Http;
using RedDog.Search.Model;
using Zbang.Zbox.ViewModel.Dto.Library;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class UniversitySearchProvider : IUniversityReadSearchProvider, IDisposable
    {
        private const string IndexName = "universities";
        private readonly ApiConnection m_Connection;
        private readonly IndexQueryClient m_ReadClient;
        public UniversitySearchProvider()
        {
            m_Connection = ApiConnection.Create(
                WriteSearchUniversityProvider.CloudentssearchSearchWindowsNet,
                WriteSearchUniversityProvider.ApiKey);

            m_ReadClient = new IndexQueryClient(m_Connection);

        }

        public void Dispose()
        {
            if (m_ReadClient != null)
            {
                m_ReadClient.Dispose();
            }
            if (m_Connection != null)
            {
                m_Connection.Dispose();
            }
        }

        public async Task<IEnumerable<UniversityByPrefixDto>> SearchUniversity(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return null;
            }
            var searchTask = m_ReadClient.SearchAsync(IndexName,
                  new SearchQuery(term + "*")
                  {
                      Select = "id,name,imageField",
                      Mode = SearchMode.Any
                  });
            Task<IApiResponse<SuggestionResult>> suggestTask = Task.FromResult<IApiResponse<SuggestionResult>>(null);
            if (term.Length >= 3)
            {
                suggestTask = m_ReadClient.SuggestAsync(IndexName,
                    new SuggestionQuery(term)
                    {
                        Fuzzy = true,
                        Select = "id,name,imageField"
                    });
            }
            await Task.WhenAll(searchTask, suggestTask);
            if (searchTask.Result.Body.Count > 0)
            {
                return searchTask.Result.Body.Records.Select(s => new UniversityByPrefixDto(
                 s.Properties["name"].ToString(),
                 s.Properties["imageField"].ToString(),
                 Convert.ToInt64(s.Properties["id"])
                 ));
            }
            return suggestTask.Result.Body.Records.Select(s => new UniversityByPrefixDto(
                s.Properties["name"].ToString(),
                s.Properties["imageField"].ToString(),
                Convert.ToInt64(s.Properties["id"])
                ));

        }


        public async Task UpdateData(IEnumerable<UniversitySearchDto> universityToUpload, IEnumerable<long> universityToDelete)
        {
            var listOfCommands = new List<IndexOperation>();
            if (universityToUpload != null)
            {
                listOfCommands.AddRange(universityToUpload.Select(s =>
                {
                    var x = new IndexOperation(IndexOperationType.MergeOrUpload, "id",
                        s.Id.ToString(CultureInfo.InvariantCulture))
                        .WithProperty("name", s.Name)
                        .WithProperty("extra4", String.Join(
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
            var client = new IndexManagementClient(m_Connection);
            var result = await client.PopulateAsync(IndexName, listOfCommands.ToArray());
        }
    }
}
