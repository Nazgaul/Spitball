
using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddFileToBoxCommand : ICommand
    {
        public AddFileToBoxCommand(long userId, 
            long boxId, 
            string blobAddressName,
            string fileName, 
            long length, 
            Guid? tabId, bool isQuestion)
        {
            IsQuestion = isQuestion;
            UserId = userId;
            BoxId = boxId;
            BlobAddressName = blobAddressName;
            FileName = fileName;
            Length = length;
            TabId = tabId;

        }

        public long UserId { get; private set; }

        public long BoxId { get; private set; }

        public string BlobAddressName { get; private set; }


        public string FileName { get; private set; }

        public long Length { get; private set; }

        public bool IsQuestion { get; private set; }



        public Guid? TabId { get; private set; }
    }
}
