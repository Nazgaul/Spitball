using System;
using Cloudents.Core.Entities;

namespace Cloudents.Command.Command
{
    public class UpdateDocumentMetaCommand : ICommand
    {
        public static UpdateDocumentMetaCommand Document(long id, int? pageCount, string snippet)
        {
            return new UpdateDocumentMetaCommand(id,pageCount,snippet,DocumentType.Document,null);
        }

        public static UpdateDocumentMetaCommand Video(long id,TimeSpan duration )
        {
            return new UpdateDocumentMetaCommand(id,null, null, DocumentType.Video, duration);
        }
        private UpdateDocumentMetaCommand(long id, 
            int? pageCount, string snippet, DocumentType documentType, TimeSpan? duration)
        {
            Id = id;
            
           // Language = language;
            PageCount = pageCount;
            Snippet = snippet;
            DocumentType = documentType;
            Duration = duration;
        }

        public long Id { get; }

        public int? PageCount { get; }

        public string Snippet { get; }

        public DocumentType DocumentType { get;  }

        public TimeSpan? Duration { get;  }

    }
}