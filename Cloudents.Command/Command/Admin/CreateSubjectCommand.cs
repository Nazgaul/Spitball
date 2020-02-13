namespace Cloudents.Command.Command.Admin
{
    public class CreateSubjectCommand : ICommand
    {
        public CreateSubjectCommand(string enSubjectName, string heSubjectName)
        {
            EnSubjectName = enSubjectName;
            HeSubjectName = heSubjectName;
        }
        public string EnSubjectName { get; set; }
        public string HeSubjectName { get; set; }
    }
}
