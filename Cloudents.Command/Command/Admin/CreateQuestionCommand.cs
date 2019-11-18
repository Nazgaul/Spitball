using System;

namespace Cloudents.Command.Command.Admin
{
    public class CreateQuestionCommand : ICommand
    {
        public CreateQuestionCommand(string courseName, Guid university,
            string text, string country)
        {
            CourseName = courseName;
            Text = text;
            Country = country;
            University = university;
        }

        public string CourseName { get; }
        public string Text { get; }


        public string Country { get; }
        public Guid University { get; }
    }
}