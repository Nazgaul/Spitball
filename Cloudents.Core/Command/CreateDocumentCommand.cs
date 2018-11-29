using System;
using System.Collections.Generic;
using System.Linq;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class CreateDocumentCommand : ICommand
    {
        public CreateDocumentCommand(string blobName, string name, DocumentType type, string course,
            IEnumerable<string> tags, long userId, string professor)
        {
            BlobName = blobName;
            Name = name;
            Type = type;
            Course = course;
            Tags = tags ?? Enumerable.Empty<string>();
            Professor = professor;
            UserId = userId;
        }



        public static CreateDocumentCommand DbiOnly(string blobName, string name, DocumentType type, string course,
            IEnumerable<string> tags, long userId, string professor, Guid universityId)
        {
            return new CreateDocumentCommand(blobName,name,type,course,tags,userId,professor)
            {
                UniversityId = universityId
            };
        }

        public string BlobName { get;  }
        public string Name { get; }
        public DocumentType Type { get;  }

        public string Course { get;  }
        public IEnumerable<string> Tags { get;  }

        public long UserId { get;  }

        public string Professor { get; set; }

        public long Id { get; set; }

        public Guid? UniversityId { get; private set; }
    }
}