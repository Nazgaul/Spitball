using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Zbox.ApiViewModel.DTOs
{
    [DataContract]
    public class FileDto : ItemDto
    {
        public FileDto(string uid, string name, DateTime creationTime, DateTime updateTime, string blobName, DateTime contentUpdateTime,
            string thumbnailBlobUrl, long commentCount,
            string userName, long userUid, string userImage, UserType userType)
            : base(name, uid, commentCount, creationTime, updateTime, userName, userUid, userImage, userType)
        {
            ContentUpdateTime = contentUpdateTime;
            BlobName = blobName;
            ThumbnailBlobUrl = thumbnailBlobUrl;
        }
        [DataMember]
        public DateTime ContentUpdateTime { get; set; }
        [DataMember]
        public override string ItemType
        {
            get { return "file"; }
            protected set { }
        }

        [DataMember]
        public string BlobName { get; set; }

        [DataMember]
        public string ThumbnailBlobUrl { get; set; }
    }
}
