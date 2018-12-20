using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
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