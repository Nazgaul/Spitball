using System.Collections.Generic;
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
            Tags = tags;
            Professor = professor;
            UserId = userId;
        }

        public string BlobName { get;  }
        public string Name { get; }
        public DocumentType Type { get;  }

        public string Course { get;  }
        public IEnumerable<string> Tags { get;  }

        public long UserId { get;  }

        public string Professor { get; set; }


    }
}