
using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddFileToBoxCommand : ICommand
    {
        public AddFileToBoxCommand(long userId, long boxId, string blobAddressName,
            string fileName, long length, 
            //string uploadId,
            Guid? tabId)
        {
            UserId = userId;
            BoxId = boxId;
            BlobAddressName = blobAddressName;
            FileName = fileName;
            //UploadId = uploadId;
            Length = length;
            TabId = tabId;

        }

        public long UserId { get; private set; }

        public long BoxId { get; set; }

        public string BlobAddressName { get; set; }


        public string FileName { get; set; }

        public long Length { get; set; }

        //public string UploadId { get; set; }



        public Guid? TabId { get; set; }
    }
}
