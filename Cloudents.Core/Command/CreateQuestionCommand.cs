using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper handle that")]
    public class CreateQuestionCommand : ICommand
    {
        public int SubjectId { get; private set; }
        public string Text { get; private set; }

        public decimal Price { get; private set; }

        public long UserId { get; private set; }

        [CanBeNull]
        public IEnumerable<string> Files { get; private set; }
    }
}