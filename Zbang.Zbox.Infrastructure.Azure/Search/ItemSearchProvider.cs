﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using RedDog.Search.Model;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class ItemSearchProvider : IItemReadSearchProvider, IItemWriteSearchProvider
    {
        private readonly string m_IndexName = "item";
        private bool CheckIndexExists = false;


        public ItemSearchProvider()
        {
            if (!RoleEnvironment.IsAvailable)
            {
                m_IndexName = m_IndexName + "-dev";
                return;
            }
            if (RoleEnvironment.IsEmulated)
            {
                m_IndexName = m_IndexName + "-dev";
            }
        }

        private const string IdField = "id";
        private const string NameField = "name";
        private const string ImageField = "image";
        private const string BoxNameField = "boxname";
        private const string ContentField = "content";
        private const string RateField = "rate";
        private const string ViewsField = "view";
        private const string UrlField = "url";
        private const string UniversityNameField = "universityname";
        private const string UniversityidField = "unidersityid";
        private const string BoxidField = "boxid";
        private const string UseridsField = "userids";

        private Index GetIndexStructure()
        {
            return new Index(m_IndexName)
                .WithStringField(IdField, f => f
                    .IsKey()
                    .IsRetrievable()
                )
                .WithStringField(NameField, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithStringField(ImageField, f => f
                    .IsRetrievable())
                .WithStringField(BoxNameField, f => f
                    .IsRetrievable())
                .WithStringField(ContentField, f => f
                    .IsRetrievable()
                    .IsSearchable())
                .WithDoubleField(RateField, f => f
                    .IsRetrievable())
                .WithIntegerField(ViewsField, f => f
                    .IsRetrievable())
                .WithStringField(UrlField, f => f
                    .IsRetrievable())
                .WithStringField(UniversityNameField, f => f
                    .IsRetrievable())
                .WithField(UniversityidField, "Edm.Int64", f => f
                    .IsFilterable())
                .WithField(BoxidField, "Edm.Int64", f => f
                    .IsFilterable())
                .WithStringCollectionField(UseridsField, f => f
                    .IsFilterable());
        }
        private async Task BuildIndex()
        {
            var response = await SeachConnection.Instance.IndexManagement.GetIndexAsync(m_IndexName);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var response2 = await SeachConnection.Instance.IndexManagement.CreateIndexAsync(GetIndexStructure());
            }
            CheckIndexExists = true;
        }

        public async Task<bool> UpdateData(IEnumerable<ItemSearchDto> itemToUpload, IEnumerable<long> itemToDelete)
        {
            if (!CheckIndexExists)
            {
                await BuildIndex();
            }
            var listOfCommands = new List<IndexOperation>();
            if (itemToUpload != null)
            {
                listOfCommands.AddRange(itemToUpload.Select(s => new IndexOperation(IndexOperationType.Upload, IdField,
                    s.Id.ToString(CultureInfo.InvariantCulture))
                    .WithProperty(NameField, s.Name)
                    .WithProperty(ImageField, s.Image)
                    .WithProperty(BoxNameField, s.BoxName)
                    .WithProperty(ContentField, s.Content)
                    .WithProperty(ViewsField, s.Views)
                    .WithProperty(RateField, s.Rate)
                    .WithProperty(UniversityNameField, s.UniversityName)
                    .WithProperty(UrlField, s.Url)
                    .WithProperty(UniversityidField, s.UniversityId)
                    .WithProperty(BoxidField, s.BoxId)
                    .WithProperty(UseridsField, s.UserIds.Select(s1 => s1.ToString(CultureInfo.InvariantCulture)))));
            }
            if (itemToDelete != null)
            {
                listOfCommands.AddRange(itemToDelete.Select(s =>
                    new IndexOperation(IndexOperationType.Delete, IdField, s.ToString(CultureInfo.InvariantCulture))
                    ));
            }
            var commands = listOfCommands.ToArray();
            if (commands.Length > 0)
            {

                var retVal = await SeachConnection.Instance.IndexManagement.PopulateAsync(m_IndexName, listOfCommands.ToArray());
                if (!retVal.IsSuccess)
                {
                    TraceLog.WriteError("On update search" + retVal.Error.Message);
                }
                return retVal.IsSuccess;
            }
            return true;
        }

        public async Task<IEnumerable<SearchItems>> SearchItem(ItemSearchQuery query)
        {
            if (string.IsNullOrEmpty(query.Term))
            {
                return null;
            }

            var searchResult = await SeachConnection.Instance.IndexQuery.SearchAsync(m_IndexName,
                new SearchQuery(query.Term + "*")
                {
                    Filter = string.Format("{0} eq {2} or {1}/any(t: t eq '{3}')", UniversityidField, UseridsField, query.UniversityId, query.UserId),
                    Top = query.RowsPerPage,
                    Skip = query.RowsPerPage * query.PageNumber
                });


            if (searchResult.Body.Records.Any())
            {
                return searchResult.Body.Records.Select(s => new SearchItems(
                    SeachConnection.ConvertToType<string>(s.Properties[ImageField]),
                    SeachConnection.ConvertToType<string>(s.Properties[NameField]),
                    SeachConnection.ConvertToType<long>(s.Properties[IdField]),
                    SeachConnection.ConvertToType<string>(s.Properties[ContentField]),
                    SeachConnection.ConvertToType<double>(s.Properties[RateField]),
                    SeachConnection.ConvertToType<int>(s.Properties[ViewsField]),
                    SeachConnection.ConvertToType<string>(s.Properties[BoxNameField]),
                    SeachConnection.ConvertToType<string>(s.Properties[UniversityNameField]),
                    SeachConnection.ConvertToType<string>(s.Properties[UrlField])));

            }

            return null;
        }


    }

    public interface IItemReadSearchProvider
    {
        Task<IEnumerable<SearchItems>> SearchItem(ItemSearchQuery query);
    }

    public interface IItemWriteSearchProvider
    {
        Task<bool> UpdateData(IEnumerable<ItemSearchDto> itemToUpload, IEnumerable<long> itemToDelete);
    }
}
