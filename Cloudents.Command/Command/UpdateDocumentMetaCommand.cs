namespace Cloudents.Command.Command
{
    public class UpdateDocumentMetaCommand : ICommand
    {
        public UpdateDocumentMetaCommand(long id, 
            int? pageCount, string snippet)
        {
            Id = id;
            
           // Language = language;
            PageCount = pageCount;
            Snippet = snippet;
        }

        public long Id { get; }

        public int? PageCount { get; }

        public string Snippet { get; }

    }
}