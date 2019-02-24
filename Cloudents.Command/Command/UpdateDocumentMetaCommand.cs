using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class UpdateDocumentMetaCommand : ICommand
    {
        public UpdateDocumentMetaCommand(long id, 
            int? pageCount, string snippet, IEnumerable<string> tags)
        {
            Id = id;
            
           // Language = language;
            PageCount = pageCount;
            Snippet = snippet;
            Tags = tags;
        }

        public long Id { get; }

        public int? PageCount { get; }

        public string Snippet { get; }

        public IEnumerable<string> Tags { get;  }
    }
}