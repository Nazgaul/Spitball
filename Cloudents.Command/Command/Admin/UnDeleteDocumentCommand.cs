namespace Cloudents.Command.Command.Admin
{
    public class UnDeleteDocumentCommand : ICommand
    {
        public UnDeleteDocumentCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}
