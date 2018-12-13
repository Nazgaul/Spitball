using System.Collections.Generic;
using Cloudents.Common;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.Command.Admin
{
    public class CreateQuestionCommand : ICommand
    {
        public CreateQuestionCommand(QuestionSubject subjectId, string text, decimal price, [CanBeNull] IEnumerable<string> files, string country)
        {
            SubjectId = subjectId;
            Text = text;
            Price = price;
            Files = files;
            Country = country;
        }

        public QuestionSubject SubjectId { get; set; }
        public string Text { get; set; }

        public decimal Price { get; set; }

        [CanBeNull]
        public IEnumerable<string> Files { get; set; }

        public  string Country { get; set; }
    }
}