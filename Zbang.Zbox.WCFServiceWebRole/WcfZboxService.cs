using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Enums;
using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.ViewModel.DTOs;
using Zbang.Zbox.ViewModel.Queries;
using Microsoft.WindowsAzure.StorageClient;
using System.IO;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.WCFServiceWebRole
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class WcfZboxService : IWcfZboxService
    {
        private IZboxService m_ZboxService;
        private IBlobProvider m_BlobProvider;
        private IThumbnailProvider m_ThumbnailProvider;

        public WcfZboxService(IZboxService zboxService, IBlobProvider blobProvider, IThumbnailProvider thumbnailProvider)
        {
            m_ZboxService = zboxService;
            m_BlobProvider = blobProvider;
            m_ThumbnailProvider = thumbnailProvider;
        }

        public IList<ViewModel.DTOs.UserBoxesDto> GetBoxes()
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("user is unathorized");

            //GetStorageQuery queryGetStorage = new GetStorageQuery(GetUserId());

            //StorageDto storageDto = m_ZboxService.GetStorage(queryGetStorage);

            //TODO - this is temp
            GetBoxesQuery queryStorage = new GetBoxesQuery(GetUserId(), BoxesQueryType.Owned);
            var boxes = m_ZboxService.GetUserBoxes(queryStorage);
            var ownedBoxes = boxes.Where(w => w.UserType == UserType.owner).ToList();
            return ownedBoxes;
        }

        public IList<ViewModel.DTOs.UserBoxesDto> GetSubscribedBoxes()
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("user is unathorized");
            GetBoxesQuery queryStorage = new GetBoxesQuery(GetUserId(), BoxesQueryType.Owned);
            var boxes = m_ZboxService.GetUserBoxes(queryStorage);
            var ownedBoxes = boxes.Where(w => w.UserType == UserType.subscribe).ToList();
            return ownedBoxes;
        }

        //  public BoxItemDto 
        public IList<ItemDto> GetBoxItemsPaged(long boxId)
        {

            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("user is unathorized");
            //Uri Url = new Uri(m_BlobProvider.BlobEndPointPublicFiles);// m_BlobProvider.BlobContainer.Uri;
            GetBoxItemsPagedQuery queryBoxItem = new GetBoxItemsPagedQuery(boxId, GetUserId());
            var items = m_ZboxService.GetBoxItemsPaged(queryBoxItem);
            List<ItemDto> result = items.ToList();
            
            return result;

        }

        public string GetBlobReadPermission(long itemid)
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("user is unathorized");
            GetItemQuery query = new GetItemQuery(GetUserId(), itemid);
            var result = m_ZboxService.GetBoxItem(query) as FileDto;

            var blob = m_BlobProvider.ZboxFileContainer().GetBlobReference(result.BlobName);
            return m_BlobProvider.GenerateSharedAccessReadPermissionBlobFiles(blob, 10);
        }


        public string GetWritePermission(string blobName)
        {
            if (!IsAuthenticated)
            {
                throw new UnauthorizedAccessException("user is unathorized");
            }
            var blob = m_BlobProvider.ZboxFileContainer().GetBlobReference(blobName);
            if (blob.Exists())
            {
                throw new ArgumentException("blob name already exists");
            }

            var writePermission = blob.GetSharedAccessSignature(new SharedAccessPolicy { Permissions = SharedAccessPermissions.Write, SharedAccessExpiryTime = DateTime.Now.AddMinutes(20) });
            var Uri = new Uri(blob.Uri, writePermission);
            return Uri.AbsoluteUri;

        }
        public long UploadFile(long boxId, string FileName, Guid blobName)
        {
            //Guid fileGuild = Guid.NewGuid();
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("user is unathorized");

            string contentType = ExtensionToMimeConvention.GetMimeType(Path.GetExtension(FileName));

            //string blobAddressUri = fileGuild.ToString();
            var blob = m_BlobProvider.ZboxFileContainer().GetBlobReference(blobName.ToString());
            //blob.UploadByteArray(fileContent);

            //blob.Properties.ContentType = contentType;
            //blob.SetProperties();
            MemoryStream ms = new MemoryStream();
            if (m_ThumbnailProvider.IsImage(Path.GetExtension(FileName)))
            {
                blob.DownloadToStream(ms);
            }
            string thumbnailBlobAddressUri = m_ThumbnailProvider.GetThumbnailBlobUrl(ms, blobName, Path.GetExtension(FileName), contentType);

            //string comment = string.Format("<a href=\"/DownloadBoxItem/{0}/{1}\">{1}</a>", blobAddressUri, FileName);

            //HttpContext.Current.Session[FileName] = new List<string>() { comment };
            AddFileToBoxCommand command = new AddFileToBoxCommand(GetUserId(), boxId, blobName.ToString(), thumbnailBlobAddressUri, FileName, blob.Properties.Length,string.Empty);            
            AddFileToBoxCommandResult result = m_ZboxService.AddFileToBox(command);

            return result.File.Id;
            
        }

        public bool DeleteBoxItem(long boxItemId, long boxId)
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("user is unathorized");


            DeleteItemCommand command = new DeleteItemCommand(boxItemId, GetUserId(), boxId);
            DeleteItemCommandResult result = m_ZboxService.DeleteItem(command);


            return true;
        }

        public bool ChangeBoxName(long BoxId, string NewBoxName)
        {
            if (!IsAuthenticated)
                throw new UnauthorizedAccessException("user is unathorized");

            ChangeBoxNameCommand command = new ChangeBoxNameCommand(BoxId, GetUserId(), NewBoxName);
            ChangeBoxNameCommandResult result = m_ZboxService.ChangeBoxName(command);

            return true;
        }

        private static bool IsAuthenticated
        {
            get
            {
                bool result = false;
                if (_Context != null)
                {
                    if (_Context.User != null)
                    {
                        result = _Context.User.Identity.IsAuthenticated;
                    }
                }
                return result;
            }
        }



        private long GetUserId()
        {
            long id = -1;
            if (!long.TryParse(_Context.User.Identity.Name, out id))
            {
                throw new UnauthorizedAccessException("User dont have valid id");
            }

            return id;
        }


        //private Guid GetUserId()
        //{
        //    MembershipUser membershipUser = Membership.GetUser(_Context.User.Identity.Name);
        //    return (Guid)membershipUser.ProviderUserKey;


        //}


        private static HttpContext _Context
        {
            get
            {
                return (HttpContext.Current != null) ? HttpContext.Current : null;
            }
        }



    }
}