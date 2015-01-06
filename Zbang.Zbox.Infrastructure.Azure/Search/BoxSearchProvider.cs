﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RedDog.Search.Model;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.BoxDtos;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class BoxSearchProvider : IBoxReadSearchProvider, IBoxWriteSearchProvider
    {
        private const string IndexName = "box";
        private const string IdField = "id";
        private const string NameField = "name";
        private const string ImageField = "image";
        private const string ProfessorField = "professor";
        private const string CourseField = "course";
        private const string UrlField = "url";
        private const string UniversityidField = "universityid";
        private const string UseridsField = "userids";
        private const string PrivacySettingsField = "PrivacySettings";

        private Index GetUniversityIndex()
        {
            return new Index(IndexName)
                .WithStringField(IdField, f => f
                    .IsKey()
                    .IsRetrievable()
                )
                .WithStringField(NameField, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(ImageField, f => f
                    .IsRetrievable())
                .WithStringField(ProfessorField, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(CourseField, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(UrlField, f => f
                    .IsRetrievable())
                .WithField(UniversityidField, "Edm.Int64", f => f
                    .IsFilterable())
                .WithStringCollectionField(UseridsField, f => f
                    .IsFilterable())
                .WithIntegerField(PrivacySettingsField, f => f.IsFilterable());

        }

        public async Task<bool> UpdateData(IEnumerable<BoxSearchDto> boxToUpload, IEnumerable<long> boxToDelete)
        {
            await BuildIndex();
            var listOfCommands = new List<IndexOperation>();
            if (boxToUpload != null)
            {
                listOfCommands.AddRange(boxToUpload.Select(s => new IndexOperation(IndexOperationType.Upload, IdField,
                    s.Id.ToString(CultureInfo.InvariantCulture))
                    .WithProperty(NameField, s.Name)
                    .WithProperty(ImageField, s.Image)
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

                var retVal = await SeachConnection.Instance.IndexManagement.PopulateAsync(IndexName, listOfCommands.ToArray());
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
            var response = await SeachConnection.Instance.IndexManagement.GetIndexAsync(IndexName);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await SeachConnection.Instance.IndexManagement.CreateIndexAsync(GetUniversityIndex());
            }
        }

        public async Task<IEnumerable<SearchBoxes>> SearchBox(BoxSearchQuery query)
        {
            if (string.IsNullOrEmpty(query.Term))
            {
                return null;
            }

            var searchResult = await SeachConnection.Instance.IndexQuery.SearchAsync(IndexName,
                new SearchQuery(query.Term + "*")
                {
                    Filter = string.Format("{0} eq {2} or {1}/any(t: t eq '{3}')", UniversityidField, UseridsField, query.UniversityId, query.UserId)
                });


            if (searchResult.Body.Records.Any())
            {
                return searchResult.Body.Records.Select(s => new SearchBoxes(
                    Convert.ToInt64(s.Properties[IdField]),
                    s.Properties[NameField].ToString(),
                    s.Properties[ImageField].ToString(),
                    s.Properties[ProfessorField] != null ? s.Properties[ProfessorField].ToString() : null,
                    s.Properties[CourseField] != null ? s.Properties[CourseField].ToString() : null,
                    s.Properties[UrlField].ToString()));
            }

            return null;
        }
    }

    public interface IBoxWriteSearchProvider
    {
        Task<bool> UpdateData(IEnumerable<BoxSearchDto> boxToUpload, IEnumerable<long> boxToDelete);
    }

    public interface IBoxReadSearchProvider
    {
        Task<IEnumerable<SearchBoxes>> SearchBox(BoxSearchQuery query);
    }
}
