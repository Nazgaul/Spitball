using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper handle that")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global" , Justification = "Automapper")]
    public class CreateQuestionCommand : ICommand
    {
        public int SubjectId { get;  set; }
        public string Text { get;  set; }

        public decimal Price { get;  set; }

        public long UserId { get;  set; }

        [CanBeNull]
        public IEnumerable<string> Files { get;  set; }
    }
}