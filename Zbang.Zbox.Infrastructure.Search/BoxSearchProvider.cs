using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using RedDog.Search.Model;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class BoxSearchProvider : IBoxReadSearchProvider, IBoxWriteSearchProvider
    {

        private readonly string m_IndexName = "box";
        private bool m_CheckIndexExists;
        private readonly ISearchConnection m_Connection;

        public BoxSearchProvider(ISearchConnection connection)
        {
            m_Connection = connection;
            if (m_Connection.IsDevelop)
            {
                m_IndexName = m_IndexName + "-dev";
            }
        }

        private const string IdField = "id";
        private const string NameField = "name";
        private const string ImageField = "image";
        private const string ProfessorField = "professor";
        private const string CourseField = "course";
        private const string UrlField = "url";
        private const string UniversityidField = "universityid";
        private const string UseridsField = "userids";
        private const string PrivacySettingsField = "PrivacySettings";

        private Index GetBoxIndex()
        {
            return new Index(m_IndexName)
                .WithStringField(IdField, f => f
                    .IsKey()
                    .IsRetrievable()
                )
                .WithStringField(NameField, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(ImageField, f => f //obsolete
                    .IsRetrievable())
                .WithStringField(ProfessorField, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(CourseField, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(UrlField, f => f
                    .IsRetrievable())
                .WithField(UniversityidField, "Edm.Int64", f => f //obsolete
                    .IsFilterable())
                //.WithStringField(UniversityidField2, f => f
                //    .IsFilterable())
                .WithStringCollectionField(UseridsField, f => f
                    .IsFilterable())
                .WithIntegerField(PrivacySettingsField,//obsolete
                    f => f.IsFilterable());

        }

        public async Task<bool> UpdateData(IEnumerable<BoxSearchDto> boxToUpload, IEnumerable<long> boxToDelete)
        {
            if (!m_CheckIndexExists)
            {
                //await BuildIndex();
            }
            var listOfCommands = new List<IndexOperation>();
            if (boxToUpload != null)
            {
                listOfCommands.AddRange(boxToUpload.Where(w => w.Url != null).Select(s => new IndexOperation(IndexOperationType.Upload, IdField,
                    s.Id.ToString(CultureInfo.InvariantCulture))
                    .WithProperty(NameField, s.Name)
                    .WithProperty(ProfessorField, s.Professor)
                    .WithProperty(CourseField, s.CourseCode)
                    .WithProperty(UrlField, s.Url)
                    .WithProperty(UniversityidField, s.UniversityId)
                    .WithProperty(PrivacySettingsField, (int)s.PrivacySettings)
                    .WithProperty(UseridsField, s.UserIds.Select(s1 => s1.ToString(CultureInfo.InvariantCulture)))));
            }
            if (boxToDelete != null)
            {
                listOfCommands.AddRange(boxToDelete.Select(s =>
                    new IndexOperation(IndexOperationType.Delete, IdField, s.ToString(CultureInfo.InvariantCulture))
                    ));
            }
            var commands = listOfCommands.ToArray();
            if (commands.Length > 0)
            {

                var retVal = await m_Connection.IndexManagement.PopulateAsync(m_IndexName, listOfCommands.ToArray());
                if (!retVal.IsSuccess)
                {
                    TraceLog.WriteError("On update search" + retVal.Error.Message);
                }
                return retVal.IsSuccess;
            }
            return true;
        }

        private async Task BuildIndex()
        {
            try
            {
                var response = await m_Connection.IndexManagement.GetIndexAsync(m_IndexName);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await m_Connection.IndexManagement.CreateIndexAsync(GetBoxIndex());
                }
                else
                {
                    var x = await m_Connection.IndexManagement.UpdateIndexAsync(GetBoxIndex());
                }
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("on box build index", ex);
            }
            m_CheckIndexExists = true;
        }

        public async Task<IEnumerable<SearchBoxes>> SearchBox(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken)
        {
            var searchResult = await m_Connection.IndexQuery.SearchAsync(m_IndexName,
                new SearchQuery(query.Term + "*")
                {
                    Filter = string.Format("{0} eq {2} or {1}/any(t: t eq '{3}')", UniversityidField, UseridsField, query.UniversityId, query.UserId),
                    Top = query.RowsPerPage,
                    Skip = query.RowsPerPage * query.PageNumber,
                    //Highlight = ProfessorField + "," + CourseField
                }, cancelToken);
            if (!searchResult.IsSuccess)
            {
                TraceLog.WriteError(string.Format("on box search model: {0} error: {1}", query,
                    searchResult.Error.Message));
                return null;
            }
            if (searchResult.Body.Records.Any())
            {
                return searchResult.Body.Records.Select(s => new SearchBoxes(
                    SeachConnection.ConvertToType<long>(s.Properties[IdField]),
                    SeachConnection.ConvertToType<string>(s.Properties[NameField]),
                    HighLightInField(s, ProfessorField),
                    HighLightInField(s, CourseField),
                    SeachConnection.ConvertToType<string>(s.Properties[UrlField]),
                    SeachConnection.ConvertToType<string>(s.Properties[NameField]))).ToList();
            }
            return null;
        }

        private string HighLightInField(SearchQueryRecord record, string field)
        {
            //string[] highLight;
            //if (record.Highlights.TryGetValue(field, out highLight))
            //{
            //    return String.Join("...", highLight);
            //}
            return SeachConnection.ConvertToType<string>(record.Properties[field]);
        }
    }

    public interface IBoxWriteSearchProvider
    {
        Task<bool> UpdateData(IEnumerable<BoxSearchDto> boxToUpload, IEnumerable<long> boxToDelete);
    }

    public interface IBoxReadSearchProvider
    {
        Task<IEnumerable<SearchBoxes>> SearchBox(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken);
    }
}
