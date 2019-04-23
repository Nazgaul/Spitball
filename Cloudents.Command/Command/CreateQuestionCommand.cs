using Cloudents.Core.Enum;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Command.Command
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper handle that")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Automapper")]
    public class CreateQuestionCommand : ICommand
    {
        public CreateQuestionCommand(QuestionSubject? subjectId, string text, long userId,
            [CanBeNull] IEnumerable<string> files,  string course)
        {
            SubjectId = subjectId;
            Text = text;
            UserId = userId;
            Files = files;
            Course = course;
        }

        public QuestionSubject? SubjectId { get; }
        public string Text { get; }


        public long UserId { get; }

        [CanBeNull]
        public IEnumerable<string> Files { get; }

        public string Course { get; }

    }
}