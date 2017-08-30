using System;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddFileToBoxCommand : AddItemToBoxCommand
    {
        public AddFileToBoxCommand(long userId, 
            long boxId, 
            string blobAddressName,
            string fileName, 
            long length,
            Guid? tabId,
             bool isQuestion)
            : base(userId, boxId)
        {
            IsQuestion = isQuestion;
            BlobAddressName = blobAddressName;
            FileName = fileName;
            Length = length;
            TabId = tabId;

        }


        public Guid? TabId { get; private set; }

        public string BlobAddressName { get; private set; }


        public string FileName { get; private set; }

        public long Length { get; private set; }

        public bool IsQuestion { get; private set; }





        public override string ResolverName => FileResolver;
    }
}
