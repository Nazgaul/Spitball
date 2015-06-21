using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Search;
using Microsoft.Azure.Search.Models;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class BoxSearchProvider : IBoxReadSearchProvider2, IBoxWriteSearchProvider2, IDisposable
    {
        private readonly string m_IndexName = "box2";
        private bool m_CheckIndexExists;
        private readonly ISearchConnection m_Connection;
        private readonly SearchIndexClient m_IndexClient;

        public BoxSearchProvider(ISearchConnection connection)
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
        private const string ProfessorField = "professor";
        private const string CourseField = "course";
        private const string UrlField = "url";
        private const string UniversityIdField = "universityId";
        private const string UserIdsField = "userId";
        private const string DepartmentField = "department";
        private const string TypeFiled = "type";

        private Index GetBoxIndex()
        {
            return new Index(m_IndexName, new[]
            {
                new Field(IdField,DataType.String) { IsKey = true, IsRetrievable = true},
                new Field(NameField,DataType.String) {IsRetrievable = true, IsSearchable = true},
                new Field(ProfessorField, DataType.String) { IsRetrievable = true, IsSearchable = true},
                new Field(CourseField, DataType.String) { IsRetrievable = true, IsSearchable = true},
                new Field(UrlField, DataType.String) { IsRetrievable = true},
                new Field(UniversityIdField, DataType.Int64) { IsFilterable = true, IsRetrievable = true },
                new Field(UserIdsField, DataType.Collection(DataType.String)) { IsFilterable = true, IsRetrievable = true },
                new Field(DepartmentField, DataType.Collection(DataType.String)) { IsSearchable = true, IsRetrievable = true},
                new Field(TypeFiled, DataType.Int32) { IsRetrievable = true}
            });
        }

        public async Task<bool> UpdateData(IEnumerable<BoxSearchDto> boxToUpload, IEnumerable<long> boxToDelete)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndex();
            }


            var listOfCommands = new List<IndexAction<BoxSearch>>();
            if (boxToUpload != null)
            {
                listOfCommands.AddRange(
                    boxToUpload.Select(s => new IndexAction<BoxSearch>(IndexActionType.MergeOrUpload, new BoxSearch
                    {
                        Course = s.CourseCode,
                        Department = s.Department.ToArray(),
                        Id = s.Id.ToString(CultureInfo.InvariantCulture),
                        Name = s.Name,
                        Professor = s.Professor,
                        Type = (int)s.Type,
                        UniversityId = s.UniversityId,
                        Url = s.Url,
                        UserId = s.UserIds.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray()

                    })));
            }
            if (boxToDelete != null)
            {
                listOfCommands.AddRange(boxToDelete.Select(s =>
                   new IndexAction<BoxSearch>(IndexActionType.Delete, new BoxSearch
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
                                    String.Join(", ", ex.IndexResponse.Results.Where(r => !r.Succeeded).Select(r => r.Key)));
                return false;
            }
            return true;
        }

        private async Task BuildIndex()
        {
            try
            {
                await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetBoxIndex());
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on box build index", ex);
            }
            m_CheckIndexExists = true;
        }
        public async Task<IEnumerable<SearchBoxes>> SearchBox(ViewModel.Queries.Search.SearchQueryMobile query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException("query");
            var result = await m_IndexClient.Documents.SearchAsync<BoxSearch>(query.Term + "*", new SearchParameters
            {
                Filter =
                    string.Format("{0} eq {2} or {1}/any(t: t eq '{3}')", UniversityIdField, UserIdsField,
                        query.UniversityId, query.UserId),
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                //HighlightFields = new[] { ProfessorField, CourseField, NameField },
                Select = new[] { IdField, NameField, ProfessorField, CourseField, TypeFiled }
            }, cancelToken);
            return result.Select(s => new SearchBoxes(
                SeachConnection.ConvertToType<long>(s.Document.Id),
                HighLightInField(s, NameField, s.Document.Name),
                HighLightInField(s, ProfessorField, s.Document.Professor),
                HighLightInField(s, CourseField, s.Document.Course),
                s.Document.Url,
                s.Document.Name,
                (BoxType)s.Document.Type.Value)
            ).ToList();
        }

        public async Task<IEnumerable<SearchBoxes>> SearchBox(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException("query");
            var result = await m_IndexClient.Documents.SearchAsync<BoxSearch>(query.Term + "*", new SearchParameters
            {
                Filter =
                    string.Format("{0} eq {2} or {1}/any(t: t eq '{3}')", UniversityIdField, UserIdsField,
                        query.UniversityId, query.UserId),
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                HighlightFields = new[] { ProfessorField, CourseField, NameField },
                Select = new[] { IdField, NameField, ProfessorField, CourseField, UrlField, TypeFiled }
            }, cancelToken);
            return result.Select(s => new SearchBoxes(
                SeachConnection.ConvertToType<long>(s.Document.Id),
                HighLightInField(s, NameField, s.Document.Name),
                HighLightInField(s, ProfessorField, s.Document.Professor),
                HighLightInField(s, CourseField, s.Document.Course),
                s.Document.Url,
                s.Document.Name,
                (BoxType)s.Document.Type.Value)
            ).ToList();
        }

        private static string HighLightInField(SearchResult<BoxSearch> record, string field, string defaultValue)
        {
            if (record.Highlights == null)
            {
                return defaultValue;
            }
            IList<string> highLight;
            if (record.Highlights.TryGetValue(field, out highLight))
            {
                return String.Join("...", highLight);
            }
            return defaultValue;
        }

        public void Dispose()
        {
            m_IndexClient.Dispose();
        }
    }


}
