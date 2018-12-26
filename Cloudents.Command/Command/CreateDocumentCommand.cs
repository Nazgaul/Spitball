﻿using Cloudents.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloudents.Command.Command
{
    public class CreateDocumentCommand : ICommand
    {
        public CreateDocumentCommand(string blobName, string name, DocumentType type, string course,
            IEnumerable<string> tags, long userId, string professor, decimal modelPrice)
        {
            BlobName = blobName;
            Name = name;
            Type = type;
            Course = course;
            Tags = tags ?? Enumerable.Empty<string>();
            Professor = professor;
            Price = modelPrice;
            UserId = userId;
        }



        public static CreateDocumentCommand DbiOnly(string blobName, string name, DocumentType type, string course,
            IEnumerable<string> tags, long userId, string professor, Guid universityId)
        {
            return new CreateDocumentCommand(blobName, name, type, course, tags, userId, professor, 0)
            {
                UniversityId = universityId
            };
        }

        public string BlobName { get; }
        public string Name { get; }
        public DocumentType Type { get; }

        public string Course { get; }
        public IEnumerable<string> Tags { get; }

        public long UserId { get; }

        public string Professor { get; set; }
        public decimal Price { get; }

        public long Id { get; set; }

        public Guid? UniversityId { get; private set; }
    }
}