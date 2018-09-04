using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Automapper Initialize")]
    public class CreateAnswerCommand : ICommand
    {
        public CreateAnswerCommand(long questionId, string text, long userId, 
            [CanBeNull] IEnumerable<string> files, string questionLink)
        {
            QuestionId = questionId;
            Text = text;
            UserId = userId;
            Files = files;
            QuestionLink = questionLink;
        }

        public long QuestionId { get; private set; }
        public string Text { get; private set; }

        public long UserId { get; private set; }

        [CanBeNull]
        public IEnumerable<string> Files { get; private set; }

        public string QuestionLink { get; private set; }

    }

    //public class CreateAnswerCommandResult : ICommandResult
    //{
    //    public CreateAnswerCommandResult(long questionId, long questionUserId)
    //    {
    //        QuestionId = questionId;
    //        QuestionUserId = questionUserId;
    //    }

    //    public long QuestionId { get; set; }
    //    public long QuestionUserId { get; set; }
    //}
}