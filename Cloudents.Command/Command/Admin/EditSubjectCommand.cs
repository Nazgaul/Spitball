
namespace Cloudents.Command.Command.Admin
{
    public class EditSubjectCommand : ICommand
    {
        public EditSubjectCommand(long subjectId, string newName)
        {
            SubjectId = subjectId;
            NewName = newName;
        }
        public long SubjectId { get; }
        public string NewName { get;  }
    }
}
