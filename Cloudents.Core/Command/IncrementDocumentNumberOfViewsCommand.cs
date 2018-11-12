using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    public class IncrementDocumentNumberOfViewsCommand  :ICommand
    {
        public IncrementDocumentNumberOfViewsCommand(long id)
        {
            Id = id;
        }

        public long Id { get; private set; }
    }
}