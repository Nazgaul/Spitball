﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RedDog.Search.Model;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class ItemSearchProvider2 : IItemReadSearchProvider, IItemWriteSearchProvider2
    {
        private readonly string m_IndexName = "item2";
        private readonly ISearchFilterProvider m_FilterProvider;
        private readonly ISearchConnection m_Connection;
        //private bool m_CheckIndexExists;


        public ItemSearchProvider2(ISearchFilterProvider filterProvider, ISearchConnection connection)
        {
            m_FilterProvider = filterProvider;
            m_Connection = connection;
            if (m_Connection.IsDevelop)
            {
                m_IndexName = m_IndexName + "-dev";
            }
        }

        private const string IdField = "id";
        private const string NameField = "name";
        private const string ImageField = "image";
        private const string BoxNameField = "boxname";
        private const string ContentField = "content";
        private const string SmallContentField = "metaconetent";
        private const string UrlField = "url";
        private const string UniversityNameField = "universityname";
        private const string UniversityidField = "unidersityid";
        private const string UseridsField = "userids";

        //private Index GetIndexStructure()
        //{
        //    if (m_Connection.IsDevelop)
        //    {
        //        return GetDevelopIndexStructure();
        //    }
        //    return new Index(m_IndexName)
        //        .WithStringField(IdField, f => f
        //            .IsKey()
        //            .IsRetrievable()
        //        )
        //        .WithStringField(NameField, f => f
        //            .IsRetrievable()
        //            .IsSearchable())
        //        .WithStringField(ImageField, f => f
        //            .IsRetrievable())
        //        .WithStringField(BoxNameField, f => f
        //            .IsRetrievable())
        //        .WithStringField(ContentField, f => f
        //            .IsSearchable()
        //        )
        //        .WithStringField(SmallContentField, f => f
        //            .IsRetrievable())
        //        .WithStringField(UrlField, f => f
        //            .IsRetrievable())
        //        .WithStringField(UniversityNameField, f => f
        //            .IsRetrievable())
        //        .WithStringField(UniversityidField, f => f
        //            .IsFilterable())
        //        .WithStringCollectionField(UseridsField, f => f
        //            .IsFilterable());
        //}
        //private Index GetDevelopIndexStructure()
        //{
        //    return new Index(m_IndexName)
        //        .WithStringField(IdField, f => f
        //            .IsKey()
        //            .IsRetrievable()
        //        )
        //        .WithStringField(NameField, f => f
        //            .IsRetrievable()
        //            .IsSearchable())
        //        .WithStringField(ImageField, f => f
        //            .IsRetrievable())
        //        .WithStringField(BoxNameField, f => f
        //            .IsRetrievable())
        //        .WithStringField(ContentField, f => f
        //            .IsSearchable()
        //        )
        //        .WithStringField(SmallContentField, f => f
        //            .IsRetrievable())
        //        .WithStringField(UrlField, f => f
        //            .IsRetrievable())
        //        .WithStringField(UniversityNameField, f => f
        //            .IsRetrievable())
        //        .WithStringField(UniversityidField, f => f
        //            .IsFilterable()
        //            .IsRetrievable())
        //        .WithStringCollectionField(UseridsField, f => f
        //            .IsFilterable()
        //            .IsRetrievable());
        //}

        //private async Task BuildIndex()
        //{
        //    try
        //    {
        //        var response = await m_Connection.IndexManagement.GetIndexAsync(m_IndexName);
        //        if (response.StatusCode == HttpStatusCode.NotFound)
        //        {
        //            await m_Connection.IndexManagement.CreateIndexAsync(GetIndexStructure());
        //        }
        //        //else
        //        //{
        //        //    var x = await SeachConnection.Instance.IndexManagement.UpdateIndexAsync(GetIndexStructure());
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        TraceLog.WriteError("on item build index", ex);
        //    }
        //    m_CheckIndexExists = true;
        //}

        public async Task<bool> UpdateData(IEnumerable<ItemSearchDto> itemToUpload, IEnumerable<long> itemToDelete)
        {
            //if (!m_CheckIndexExists)
            //{
            //    //if (m_Connection.IsDevelop)
            //    //{
            //    //    await m_Connection.IndexManagement.DeleteIndexAsync(m_IndexName);
            //    //}
            //    //await BuildIndex();
            //}
            var listOfCommands = new List<IndexOperation>();
            if (itemToUpload != null)
            {
                foreach (var item in itemToUpload)
                {

                    listOfCommands.Add(
                        new IndexOperation(IndexOperationType.Upload, IdField,
                            item.Id.ToString(CultureInfo.InvariantCulture))
                            .WithProperty(NameField, Path.GetFileNameWithoutExtension(item.Name))
                            .WithProperty(ImageField, item.Image)
                            .WithProperty(BoxNameField, item.BoxName)
                            .WithProperty(UniversityNameField, item.UniversityName)
                            .WithProperty(ContentField, item.Content)
                            .WithProperty(SmallContentField, item.Content.RemoveEndOfString(SeachConnection.DescriptionLength))
                            .WithProperty(UrlField, item.Url)
                            .WithProperty(UniversityidField, item.UniversityId.ToString())
                            .WithProperty(UseridsField,
                                item.UserIds.Select(s1 => s1.ToString(CultureInfo.InvariantCulture))));
                }

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
                try
                {
                    var retVal = await m_Connection.IndexManagement.PopulateAsync(m_IndexName, listOfCommands.ToArray());
                    if (!retVal.IsSuccess)
                    {
                        TraceLog.WriteError("On update search" + retVal.Error.Message);
                    }
                    return retVal.IsSuccess;
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError("on item populate async", ex);
                    return false;
                }
            }
            return true;
        }

        public async Task<IEnumerable<SearchItems>> SearchItem(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken)
        {
            if (query == null) throw new ArgumentNullException("query");
           
            var filter = await m_FilterProvider.BuildFilterExpression(
                query.UniversityId, UniversityidField, UseridsField, query.UserId);
            var searchResult = await m_Connection.IndexQuery.SearchAsync(m_IndexName,
                new SearchQuery(query.Term)
                {
                    Filter = filter,
                    Top = query.RowsPerPage,
                    ScoringProfile = "university",
                    ScoringParameters = new[] { "university:" + query.UniversityId },
                    Skip = query.RowsPerPage * query.PageNumber,
                    //Highlight = ContentField
                }, cancelToken);
            if (searchResult == null)
            {
                TraceLog.WriteError(string.Format("on item search model: {0} return null", query));
                return null;
            }
            if (!searchResult.IsSuccess)
            {
                TraceLog.WriteError(string.Format("on item search model: {0} error: {1}", query,
                    searchResult.Error.Message));
                return null;
            }

            if (searchResult.Body.Records.Any())
            {
                return searchResult.Body.Records.Select(s =>
                {
                    //string[] highLight;
                    string content = //s.Highlights.TryGetValue(ContentField, out highLight)
                        //? SeachConnection.LimitContentHighlight(highLight)
                        //    : 
                        WebUtility.HtmlEncode(SeachConnection.ConvertToType<string>(s.Properties[SmallContentField]));

                    return new SearchItems(
                        SeachConnection.ConvertToType<string>(s.Properties[ImageField]),
                        //HighLightInName(s),
                        SeachConnection.ConvertToType<string>(s.Properties[NameField]),
                        SeachConnection.ConvertToType<long>(s.Properties[IdField]),
                        content,
                        SeachConnection.ConvertToType<string>(s.Properties[BoxNameField]),
                        SeachConnection.ConvertToType<string>(s.Properties[UniversityNameField]),
                        SeachConnection.ConvertToType<string>(s.Properties[UrlField]));
                }).ToList();
            }
            return null;
        }

        //private string HighLightInName(SearchQueryRecord record)
        //{
        //    string[] highLight;
        //    if (record.Highlights.TryGetValue(NameField, out highLight))
        //    {
        //        return String.Join("...", highLight);
        //    }
        //    return SeachConnection.ConvertToType<string>(record.Properties[NameField]);
        //}


    }

}
