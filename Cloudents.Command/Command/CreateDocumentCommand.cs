﻿//using System;
//using Cloudents.Core.Enum;

//namespace Cloudents.Command.Command
//{
//    public class CreateDocumentCommand : ICommand
//    {
//        public CreateDocumentCommand(string blobName, string name, string course,
//            long userId, string? modelDescription)
//        {
//            BlobName = blobName ?? throw new ArgumentNullException(nameof(blobName));
//            Name = name ?? throw new ArgumentNullException(nameof(name));
//            Course = course ?? throw new ArgumentNullException(nameof(course));
//          //  Price = modelPrice;
//            ModelDescription = modelDescription;
//           // PriceType = priceType;
//            UserId = userId;
//        }



//        public string BlobName { get; }
//       public string Name { get; }

//        public string Course { get; }

//        public long UserId { get; }

//        public string? ModelDescription { get; }

//    }
//}