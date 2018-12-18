using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class UpdateDocumentMetaCommand : ICommand
    {
        public UpdateDocumentMetaCommand(long id, 
            //CultureInfo language,
            int? pageCount, string snippet)
        {
            Id = id;
            
           // Language = language;
            PageCount = pageCount;
            Snippet = snippet;
        }

        public long Id { get; }

        //public CultureInfo Language { get; }

        public int? PageCount { get; }

        public string Snippet { get; private set; }
    }
}