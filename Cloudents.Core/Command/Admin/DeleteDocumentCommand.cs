using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
{
    public class DeleteDocumentCommand : ICommand
    {
        public DeleteDocumentCommand(long id)
        {
            Id = id;
        }

        public long Id { get; private set; }
    }
}