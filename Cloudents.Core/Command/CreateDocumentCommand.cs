using System.Collections.Generic;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class CreateDocumentCommand : ICommand
    {
        public CreateDocumentCommand(string blobName, string name, DocumentType type, IEnumerable<string> courses, IEnumerable<string> tags, long userId)
        {
            BlobName = blobName;
            Name = name;
            Type = type;
            Courses = courses;
            Tags = tags;
            UserId = userId;
        }

        public string BlobName { get;  }
        public string Name { get; }
        public DocumentType Type { get;  }

        public IEnumerable<string> Courses { get;  }
        public IEnumerable<string> Tags { get;  }

        public long UserId { get;  }


    }
}