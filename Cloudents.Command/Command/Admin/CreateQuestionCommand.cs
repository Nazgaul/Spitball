namespace Cloudents.Command.Command.Admin
{
    public class CreateQuestionCommand : ICommand
    {
        public CreateQuestionCommand(string courseName, 
            string text, string country)
        {
            CourseName = courseName;
            Text = text;
            Country = country;
        }

        public string CourseName { get; }
        public string Text { get; }


        public string Country { get; }
    }
}