using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using RedDog.Search.Model;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Search;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class ItemSearchProvider : IItemWriteSearchProvider
    {
        private readonly string m_IndexName = "item";
        private readonly IBlobProvider m_BlobProvider;

        private readonly ISearchConnection m_Connection;


        public ItemSearchProvider(IBlobProvider blobProvider, ISearchConnection connection)
        {
            m_BlobProvider = blobProvider;
            m_Connection = connection;
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

        //private Index GetIndexStructure()
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
        //            .IsRetrievable()
        //            .IsSearchable())
        //        .WithDoubleField(RateField, f => f
        //            .IsRetrievable())
        //        .WithIntegerField(ViewsField, f => f
        //            .IsRetrievable())
        //        .WithStringField(UrlField, f => f
        //            .IsRetrievable())
        //        .WithStringField(UniversityNameField, f => f
        //            .IsRetrievable())
        //        .WithField(UniversityidField, "Edm.Int64", f => f
        //            .IsFilterable())
        //        .WithField(BoxidField, "Edm.Int64", f => f
        //            .IsFilterable())
        //        .WithStringCollectionField(UseridsField, f => f
        //            .IsFilterable());
        //}
        //private async Task BuildIndex()
        //{
        //    var response = await SeachConnection.Instance.IndexManagement.GetIndexAsync(m_IndexName);
        //    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        //    {
        //        await SeachConnection.Instance.IndexManagement.CreateIndexAsync(GetIndexStructure());
        //    }
        //}

        public async Task<string> FetchContent(ItemSearchDto itemToUpload)
        {
            try
            {
                var metaData = await m_BlobProvider.FetechBlobMetaDataAsync(itemToUpload.BlobName);
                string content;
                if (metaData.TryGetValue(StorageConsts.ContentMetaDataKey, out content))
                {
                    var retVal = DecodeText(content).Trim();

                    if (string.IsNullOrEmpty(retVal))
                    {
                        return itemToUpload.Content;
                    }

                    var x = new Regex("%.");
                    if (x.IsMatch(retVal, retVal.Length - 2))
                    {
                        retVal = retVal.Substring(0, (retVal.Length - 2));
                    }
                    retVal = retVal.TrimEnd("&qu");
                    retVal = retVal.TrimEnd((char)65533, '%');
                    return retVal;
                }
                return itemToUpload.Content;
            }
            catch (Exception ex)
            {
                TraceLog.WriteError("problem with getting content of " + itemToUpload.Id, ex);
                return itemToUpload.Content;
            }
        }

        private string DecodeText(string val)
        {
            while (WebUtility.UrlDecode(val) != val)
            {
                val = WebUtility.UrlDecode(val);
            }
            return val;

        }

        public async Task<bool> UpdateData(IEnumerable<ItemSearchDto> itemToUpload, IEnumerable<long> itemToDelete)
        {
            //if (!m_CheckIndexExists)
            //{
            //    await BuildIndex();
            //}
            var listOfCommands = new List<IndexOperation>();
            if (itemToUpload != null)
            {
                foreach (var item in itemToUpload)
                {
                    var content = await FetchContent(item);
                    listOfCommands.Add(
                        new IndexOperation(IndexOperationType.Upload, IdField,
                            item.Id.ToString(CultureInfo.InvariantCulture))
                            .WithProperty(NameField, item.Name)
                            .WithProperty(ImageField, item.Image)
                            .WithProperty(BoxNameField, item.BoxName)
                            .WithProperty(ContentField, content)
                            .WithProperty(ViewsField, item.Views)
                            .WithProperty(RateField, item.Rate)
                            .WithProperty(UniversityNameField, item.UniversityName)
                            .WithProperty(UrlField, item.Url)
                            .WithProperty(UniversityidField, item.UniversityId)
                            .WithProperty(BoxidField, item.BoxId)
                            .WithProperty(UseridsField,
                                item.UserIds.Select(s1 => s1.ToString(CultureInfo.InvariantCulture))));
                }
                //listOfCommands.AddRange(itemToUpload.Select(async s => 
                //   new IndexOperation(IndexOperationType.Upload, IdField,
                //    s.Id.ToString(CultureInfo.InvariantCulture))
                //    .WithProperty(NameField, s.Name)
                //    .WithProperty(ImageField, s.Image)
                //    .WithProperty(BoxNameField, s.BoxName)
                //    .WithProperty(ContentField, await FetchContent(s))
                //    .WithProperty(ViewsField, s.Views)
                //    .WithProperty(RateField, s.Rate)
                //    .WithProperty(UniversityNameField, s.UniversityName)
                //    .WithProperty(UrlField, s.Url)
                //    .WithProperty(UniversityidField, s.UniversityId)
                //    .WithProperty(BoxidField, s.BoxId)
                //    .WithProperty(UseridsField, s.UserIds.Select(s1 => s1.ToString(CultureInfo.InvariantCulture)))));
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

        


    }

    public interface IItemReadSearchProvider
    {
        Task<IEnumerable<SearchItems>> SearchItem(ViewModel.Queries.Search.SearchQuery query, CancellationToken cancelToken);
    }

    public interface IItemWriteSearchProvider
    {
        Task<bool> UpdateData(IEnumerable<ItemSearchDto> itemToUpload, IEnumerable<long> itemToDelete);
    }

    public interface IItemWriteSearchProvider2 : IItemWriteSearchProvider
    {
    }
}
