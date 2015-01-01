using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RedDog.Search;
using RedDog.Search.Http;
using RedDog.Search.Model;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.Library;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class UniversitySearchProvider : IUniversityReadSearchProvider, IDisposable, IUniversityWriteSearchProvider2
    {
        private const string IndexName = "universities2";
        private readonly ApiConnection m_Connection;
        private IndexQueryClient m_ReadClient;
        private IndexManagementClient m_IndexClient;
        public UniversitySearchProvider()
        {
            m_Connection = ApiConnection.Create(
                ConfigFetcher.Fetch("AzureSeachServiceName"),
                ConfigFetcher.Fetch("AzureSearchKey")
                );
        }

        private async Task BuildIndex()
        {
            if (m_IndexClient == null)
            {
                m_IndexClient = new IndexManagementClient(m_Connection);
            }
            var response = await m_IndexClient.GetIndexAsync(IndexName);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await m_IndexClient.CreateIndexAsync(GetUniversityIndex());
            }

        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }

        private void Dispose(bool p)
        {
            if (m_ReadClient != null)
            {
                m_ReadClient.Dispose(p);
            }
            if (m_IndexClient != null)
            {
                m_IndexClient.Dispose(p);
            }
            if (m_Connection != null)
            {
                m_Connection.Dispose(p);
            }
        }

        public async Task<IEnumerable<UniversityByPrefixDto>> SearchUniversity(UniversitySearchQuery query)
        {

            if (string.IsNullOrEmpty(query.Term))
            {
                return null;
            }
            if (m_ReadClient == null)
            {
                m_ReadClient = new IndexQueryClient(m_Connection);
            }
            var searchTask = m_ReadClient.SearchAsync(IndexName,
                  new SearchQuery(query.Term + "*")
                  {
                      Select = "id,name,imageField",
                      Top = query.RowsPerPage,
                      Skip = query.RowsPerPage * query.PageNumber
                  });
            var suggestTask = Task.FromResult<IApiResponse<SuggestionResult>>(null);
            if (query.Term.Length >= 3)
            {
                suggestTask = m_ReadClient.SuggestAsync(IndexName,
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
                if (m_IndexClient == null)
                {
                    m_IndexClient = new IndexManagementClient(m_Connection);
                }
                var retVal = await m_IndexClient.PopulateAsync(IndexName, listOfCommands.ToArray());
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
