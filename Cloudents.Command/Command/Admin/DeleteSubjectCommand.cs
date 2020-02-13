
namespace Cloudents.Command.Command.Admin
{
    public class DeleteSubjectCommand : ICommand
    {
        public DeleteSubjectCommand(long subjectId)
        {
            SubjectId = subjectId;
        }
        public long SubjectId { get; }
    }
}
