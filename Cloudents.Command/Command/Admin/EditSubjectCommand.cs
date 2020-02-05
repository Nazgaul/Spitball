
namespace Cloudents.Command.Command.Admin
{
    public class EditSubjectCommand : ICommand
    {
        public EditSubjectCommand(long subjectId, string enSubjectName, string heSubjectName)
        {
            SubjectId = subjectId;
            EnSubjectName = enSubjectName;
            HeSubjectName = heSubjectName;
        }
        public long SubjectId { get; set; }
        public string EnSubjectName { get; set; }
        public string HeSubjectName { get; set; }
    }
}
