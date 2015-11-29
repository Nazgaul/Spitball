﻿
using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class AddFileToBoxCommand : AddItemToBoxCommand
    {
        public AddFileToBoxCommand(long userId, 
            long boxId, 
            string blobAddressName,
            string fileName, 
            long length,
             bool isQuestion)
            : base(userId, boxId)
        {
            IsQuestion = isQuestion;
            BlobAddressName = blobAddressName;
            FileName = fileName;
            Length = length;

        }

       


        public string BlobAddressName { get; private set; }


        public string FileName { get; private set; }

        public long Length { get; private set; }

        public bool IsQuestion { get; private set; }





        public override string ResolverName
        {
            get { return FileResolver; }
        }
    }
}
