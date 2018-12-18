using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Common;
using Cloudents.Core.Interfaces;
using Cloudents.Domain.Enums;
using JetBrains.Annotations;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper handle that")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global" , Justification = "Automapper")]
    public class CreateQuestionCommand : ICommand
    {
        public CreateQuestionCommand(QuestionSubject subjectId, string text, decimal price, long userId, 
            [CanBeNull] IEnumerable<string> files,  QuestionColor color)
        {
            SubjectId = subjectId;
            Text = text;
            Price = price;
            UserId = userId;
            Files = files;
            Color = color;
        }

        public QuestionSubject SubjectId { get;  set; }
        public string Text { get;  set; }

        public decimal Price { get;  set; }

        public long UserId { get;  set; }

        [CanBeNull]
        public IEnumerable<string> Files { get;  set; }

        //TODO : remove this
        public long Id { get; set; }

        public QuestionColor Color { get; set; }

    }
}