using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
{
    public class ApproveDocumentCommand : ICommand
    {
        public ApproveDocumentCommand(long id)
        {
            Id = id;
        }

        public long Id { get;private set; }
    }
}