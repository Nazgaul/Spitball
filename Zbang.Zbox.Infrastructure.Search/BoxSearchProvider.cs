﻿using System;
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
        private readonly SearchIndexClient m_IndexClient;

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
        private const string DepartmentField = "department";
        private const string TypeFiled = "type";
        private const string FeedField = "feed";

        private const string MembersField = "membersCount";
        private const string ItemsField = "itemsCount";
        private const string DepartmentIdField = "departmentId";



        //private const string ParentDepartmentField = "parentDepartment";

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
                new Field(TypeFiled, DataType.Int32) { IsRetrievable = true},
                new Field(FeedField, DataType.Collection(DataType.String)) { IsSearchable = true, IsRetrievable = true},
                new Field("parentDepartment", DataType.String) { IsRetrievable = true ,IsSortable = true, IsSearchable = true},
                new Field(DepartmentIdField, DataType.String) { IsFilterable = true, IsRetrievable = true },

                new Field(MembersField, DataType.Int32) {  IsRetrievable = true },
                new Field(ItemsField, DataType.Int32) {  IsRetrievable = true }
            });
        }

        public async Task<bool> UpdateDataAsync(IEnumerable<BoxSearchDto> boxToUpload, IEnumerable<long> boxToDelete)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndexAsync();
            }
            var t1 = Extensions.TaskExtensions.CompletedTask;
            var t2 = Extensions.TaskExtensions.CompletedTask;
            if (boxToUpload != null)
            {
                var uploadBatch = boxToUpload.Select(s => new BoxSearch
                {
                    Course = s.CourseCode,
                    Department = s.Department.ToArray(),
                    Feed = s.Feed.ToArray(),
                    DepartmentId = s.DepartmentId?.ToString(),
                    Id = s.Id.ToString(CultureInfo.InvariantCulture),
                    Name = s.Name,
                    Professor = s.Professor,
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
            await Task.WhenAll(t1, t2);
            return true;
        }

        private async Task BuildIndexAsync()
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
                Select = new[] { IdField, NameField, ProfessorField, CourseField, UrlField, TypeFiled,DepartmentIdField , MembersField ,ItemsField},
            }, cancellationToken: cancelToken);
            return result.Results.Select(s => new SearchBoxes(
                SeachConnection.ConvertToType<long>(s.Document.Id),
                s.Document.Name,
                s.Document.Professor,
                s.Document.Course,
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
