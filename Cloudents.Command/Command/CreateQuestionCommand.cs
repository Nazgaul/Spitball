using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Command.Command
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper handle that")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Automapper")]
    public class CreateQuestionCommand : ICommand
    {
        public CreateQuestionCommand( string text, long userId,
             string course)
        {
            Text = text;
            UserId = userId;
            Course = course;
        }

        public string Text { get; }


        public long UserId { get; }


        public string Course { get; }

    }
}