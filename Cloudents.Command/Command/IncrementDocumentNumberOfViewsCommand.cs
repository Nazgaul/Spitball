namespace Cloudents.Command.Command
{
    public class IncrementDocumentNumberOfViewsCommand : ICommand
    {
        public IncrementDocumentNumberOfViewsCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }

    public class IncrementDocumentNumberOfDownloadsCommand : ICommand
    {
        public IncrementDocumentNumberOfDownloadsCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}