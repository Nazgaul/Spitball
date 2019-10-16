using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Command.Command
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Automapper Initialize")]
    public class CreateAnswerCommand : ICommand
    {
        public CreateAnswerCommand(long questionId, string text, long userId)
        {
            QuestionId = questionId;
            Text = text;
            UserId = userId;
           
        }

        public long QuestionId { get; }
        public string Text { get; }

        public long UserId { get; }

        //[CanBeNull]
        //public IEnumerable<string> Files { get; }

    }
   
}