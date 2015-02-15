using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using RedDog.Search.Model;
using Zbang.Zbox.Infrastructure.Consts;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ViewModel.Dto.ItemDtos;
using Zbang.Zbox.ViewModel.Dto.Search;
using Zbang.Zbox.ViewModel.Queries.Search;

namespace Zbang.Zbox.Infrastructure.Azure.Search
{
    public class ItemSearchProvider : IItemReadSearchProvider, IItemWriteSearchProvider
    {
        private readonly string m_IndexName = "item";
        private readonly IBlobProvider m_BlobProvider;
        private readonly ISearchFilterProvider m_FilterProvider;
        private bool m_CheckIndexExists;


        public ItemSearchProvider(IBlobProvider blobProvider, ISearchFilterProvider filterProvider)
        {
            m_BlobProvider = blobProvider;
            m_FilterProvider = filterProvider;
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
        private const string SmallContentField = "metaconetent";
        private const string UrlField = "url";
        private const string UniversityNameField = "universityname";
        private const string UniversityidField = "unidersityid";
        private const string UniversityidField2 = "unidersityid2";
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
                    .IsSearchable())
                .WithStringField(SmallContentField, f => f
                    .IsRetrievable())
                .WithStringField(UrlField, f => f
                    .IsRetrievable())
                .WithStringField(UniversityNameField, f => f
                    .IsRetrievable())
                .WithField(UniversityidField, "Edm.Int64", f => f //obsolete
                    .IsFilterable())
                .WithStringField(UniversityidField2, f => f
                    .IsFilterable())
                .WithStringCollectionField(UseridsField, f => f
                    .IsFilterable());
        }
        private async Task BuildIndex()
        {
            var response = await SeachConnection.Instance.IndexManagement.GetIndexAsync(m_IndexName);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await SeachConnection.Instance.IndexManagement.CreateIndexAsync(GetIndexStructure());
            }
            else
            {
                await SeachConnection.Instance.IndexManagement.UpdateIndexAsync(GetIndexStructure());
            }
            m_CheckIndexExists = true;
        }

        public async Task<string> FetchContent(ItemSearchDto itemToUpload)
        {
            try
            {
                var metaData = await m_BlobProvider.FetechBlobMetaDataAsync(itemToUpload.BlobName);
                string content;
                if (metaData.TryGetValue(StorageConsts.ContentMetaDataKey, out content))
                {
                    var retVal = DecodeText(content, itemToUpload.Id).Trim();

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

        private string DecodeText(string val, long itemId)
        {
            var initVal = val;
            var i = 0;
            while (HttpUtility.UrlDecode(val) != val)
            {

                val = HttpUtility.UrlDecode(val);
                i++;
            }
            if (i > 1)
            {
                TraceLog.WriteInfo("val is encoded couple of times i: " + i + " itemid: " + itemId + " val " + initVal);
            }

            return val;

        }

        public async Task<bool> UpdateData(IEnumerable<ItemSearchDto> itemToUpload, IEnumerable<long> itemToDelete)
        {
            if (!m_CheckIndexExists)
            {
                await BuildIndex();
            }
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
                            .WithProperty(UniversityNameField, item.UniversityName)
                            .WithProperty(ContentField, content)
                            .WithProperty(SmallContentField, content.RemoveEndOfString(SeachConnection.DescriptionLength))
                            .WithProperty(UrlField, item.Url)
                            .WithProperty(UniversityidField, item.UniversityId) //obsolete
                            .WithProperty(UniversityidField2, item.UniversityId.ToString())
                        //.WithProperty(BoxidField, item.BoxId)
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

                var retVal = await SeachConnection.Instance.IndexManagement.PopulateAsync(m_IndexName, listOfCommands.ToArray());
                if (!retVal.IsSuccess)
                {
                    TraceLog.WriteError("On update search" + retVal.Error.Message);
                }
                return retVal.IsSuccess;
            }
            return true;
        }

        public async Task<IEnumerable<SearchItems>> SearchItem(Zbang.Zbox.ViewModel.Queries.Search.SearchQuery query)
        {
            if (string.IsNullOrEmpty(query.Term))
            {
                return null;
            }

            var searchResult = await SeachConnection.Instance.IndexQuery.SearchAsync(m_IndexName,
                new RedDog.Search.Model.SearchQuery(query.Term + "*")
                {
                    //Filter = await m_FilterProvider.BuildFilterExpression(
                    //   query.UniversityId, UniversityidField, UseridsField, query.UserId),
                    Filter = string.Format("{0} eq {2} or {1}/any(t: t eq '{3}')",
                      UniversityidField,
                      UseridsField,
                      query.UniversityId,
                      query.UserId),

                    Top = query.RowsPerPage,
                    //ScoringProfile = "university",
                    //ScoringParameters = new[] { "university:" + query.UniversityId },
                    Skip = query.RowsPerPage * query.PageNumber,
                    Highlight = ContentField + "," + NameField,
                    SearchFields = string.Join(",", new[] { ImageField, NameField, IdField, SmallContentField, BoxNameField, UniversityNameField, UrlField })
                });


            if (searchResult.Body.Records.Any())
            {
                return searchResult.Body.Records.Select(s =>
                {
                    string content = string.Empty;
                    string[] highLight;
                    if (s.Highlights.TryGetValue(ContentField, out highLight))
                    {
                        content = SeachConnection.LimitContentHighlight(highLight);
                    }
                    else
                    {
                        content = SeachConnection.ConvertToType<string>(s.Properties[SmallContentField]);
                    }

                    return new SearchItems(
                        SeachConnection.ConvertToType<string>(s.Properties[ImageField]),
                        SeachConnection.ConvertToType<string>(s.Properties[NameField]),
                        SeachConnection.ConvertToType<long>(s.Properties[IdField]),
                        content,
                        SeachConnection.ConvertToType<string>(s.Properties[BoxNameField]),
                        SeachConnection.ConvertToType<string>(s.Properties[UniversityNameField]),
                        SeachConnection.ConvertToType<string>(s.Properties[UrlField]));
                });
            }
            return null;
        }


    }

    public interface IItemReadSearchProvider
    {
        Task<IEnumerable<SearchItems>> SearchItem(Zbang.Zbox.ViewModel.Queries.Search.SearchQuery query);
    }

    public interface IItemWriteSearchProvider
    {
        Task<bool> UpdateData(IEnumerable<ItemSearchDto> itemToUpload, IEnumerable<long> itemToDelete);
    }
}
