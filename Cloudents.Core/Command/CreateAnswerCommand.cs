using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper handle that")]
    public class CreateAnswerCommand : ICommand
    {
        public CreateAnswerCommand(long questionId, string text, long userId, [CanBeNull] IEnumerable<string> files)
        {
            QuestionId = questionId;
            Text = text;
            UserId = userId;
            Files = files;
        }

        public CreateAnswerCommand()
        {
            
        }

        public long QuestionId { get; private set; }
        public string Text { get; private set; }

        public long UserId { get; private set; }

        [CanBeNull]
        public IEnumerable<string> Files { get; private set; }
    }
}