using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Search;
using Microsoft.Azure.Search.Models;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class BoxSearchProvider : IBoxReadSearchProvider2, IBoxWriteSearchProvider2, IDisposable
    {
        private readonly string m_IndexName = "box2";
        private bool m_CheckIndexExists;
        private readonly ISearchConnection m_Connection;
        private readonly ISearchIndexClient m_IndexClient;

        public BoxSearchProvider(ISearchConnection connection)
        {
            m_Connection = connection;
            if (connection.IsDevelop)
            {
                m_IndexName = m_IndexName + "-dev";
            }
            //TraceLog.WriteInfo("index name " + m_IndexName);
            m_IndexClient = connection.SearchClient.Indexes.GetClient(m_IndexName);
        }

        private const string IdField = "id";
        private const string NameField = "name";
        private const string ProfessorField = "professor";
        private const string CourseField = "course";
        private const string UrlField = "url";
        private const string UniversityIdField = "universityId";
        private const string UserIdsField = "userId";
        private const string TypeFiled = "type";

        private const string MembersField = "membersCount";
        private const string ItemsField = "itemsCount";
        private const string DepartmentIdField = "departmentId";

        private Index GetBoxIndex()
        {
            var suggester = new Suggester("sg", SuggesterSearchMode.AnalyzingInfixMatching,
                nameof(BoxSearch.Name2).ToLowerInvariant(), nameof(BoxSearch.Professor2).ToLowerInvariant(), nameof(BoxSearch.Course2).ToLowerInvariant());
            var index = new Index
            {
                Name = m_IndexName,
                Fields = FieldBuilder.BuildForType<BoxSearch>(),
                Suggesters = new[] {suggester}
            };
            return index;
            



        }

        public async Task<bool> UpdateDataAsync(IEnumerable<BoxSearchDto> boxToUpload, IEnumerable<long> boxToDelete)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndexAsync().ConfigureAwait(false);
            }
            var t1 = Task.CompletedTask;
            var t2 = Task.CompletedTask;
            if (boxToUpload != null)
            {
                var uploadBatch = boxToUpload.Select(s => new BoxSearch
                {
#pragma warning disable 618 // we need to popluate this
                    Course = s.CourseCode,

                    Name = s.Name,
                    Professor = s.Professor,
#pragma warning restore 618
                    Course2 = s.CourseCode,
                    Name2 = s.Name,
                    Professor2 = s.Professor,

                    Department = s.Department.ToArray(),
                    Feed = s.Feed.ToArray(),
                    DepartmentId = s.DepartmentId?.ToString(),
                    Id = s.Id.ToString(CultureInfo.InvariantCulture),
                   
                    Type = (int)s.Type,
                    UniversityId = s.UniversityId,
                    Url = s.Url,
                    ItemsCount = s.ItemsCount,
                    MembersCount = s.MembersCount,
                    UserId = s.UserIds.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray()
                });

                var batch = IndexBatch.Upload(uploadBatch);
                if (batch.Actions.Any())
                {
                    t1 = m_IndexClient.Documents.IndexAsync(batch);
                }
            }
            if (boxToDelete != null)
            {
                var deleteBatch = boxToDelete.Select(s =>
                    new BoxSearch
                    {
                        Id = s.ToString(CultureInfo.InvariantCulture)
                    });
                var batch = IndexBatch.Delete(deleteBatch);
                if (batch.Actions.Any())
                {
                    t2 = m_IndexClient.Documents.IndexAsync(batch);
                }
            }
            await Task.WhenAll(t1, t2).ConfigureAwait(false);
            return true;
        }

        private async Task BuildIndexAsync()
        {
            try
            {
                await m_Connection.SearchClient.Indexes.CreateOrUpdateAsync(GetBoxIndex()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on box build index", ex);
            }
            m_CheckIndexExists = true;
        }

        public async Task<IEnumerable<SearchBoxes>> SearchBoxAsync(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            var result = await m_IndexClient.Documents.SearchAsync<BoxSearch>(query.Term + "*", new SearchParameters
            {
                Filter =
                    string.Format("{0} eq {2} or {1}/any(t: t eq '{3}')", UniversityIdField, UserIdsField,
                        query.UniversityId, query.UserId),
                Top = query.RowsPerPage,
                Skip = query.RowsPerPage * query.PageNumber,
                Select = new[] { IdField, nameof(BoxSearch.Course2).ToLower(), nameof(BoxSearch.Professor2).ToLower(), nameof(BoxSearch.Name2).ToLower(), UrlField, TypeFiled,DepartmentIdField , MembersField ,ItemsField},
            }, cancellationToken: cancelToken).ConfigureAwait(false);
            return result.Results.Select(s => new SearchBoxes(
                SeachConnection.ConvertToType<long>(s.Document.Id),
                s.Document.Name2,
                s.Document.Professor2,
                s.Document.Course2,
                s.Document.Url,
                s.Document.DepartmentId,
                s.Document.ItemsCount.GetValueOrDefault(),
                s.Document.MembersCount.GetValueOrDefault(),
                (BoxType)s.Document.Type.GetValueOrDefault()
                )
            ).ToList();
        }
        public void Dispose()
        {
            m_IndexClient.Dispose();
        }
    }


}
