using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper handle that")]
    public class CreateQuestionCommand : ICommand
    {
        //public CreateQuestionCommand(int subjectId, string text, decimal price, long userId)
        //{
        //    SubjectId = subjectId;
        //    Text = text;
        //    Price = price;
        //    UserId = userId;
        //}

        public int SubjectId { get; private set; }
        public string Text { get; private set; }

        public decimal Price { get; private set; }

        public long UserId { get; private set; }

        public IEnumerable<string> Files { get; private set; }
    }
}