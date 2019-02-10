namespace Cloudents.Command.Command.Admin
{
    public class DeleteDocumentCommand : ICommand
    {
        public DeleteDocumentCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}