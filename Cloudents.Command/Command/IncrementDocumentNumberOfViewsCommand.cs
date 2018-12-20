using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command
{
    public class IncrementDocumentNumberOfViewsCommand  :ICommand
    {
        public IncrementDocumentNumberOfViewsCommand(long id)
        {
            Id = id;
        }

        public long Id { get; private set; }
    }

    public class IncrementDocumentNumberOfDownloadsCommand : ICommand
    {
        public IncrementDocumentNumberOfDownloadsCommand(long id)
        {
            Id = id;
        }

        public long Id { get; private set; }
    }
}