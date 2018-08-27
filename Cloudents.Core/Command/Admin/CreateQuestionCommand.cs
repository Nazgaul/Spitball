using System.Collections.Generic;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;

namespace Cloudents.Core.Command.Admin
{
    public class CreateQuestionCommand : ICommand
    {
        public int SubjectId { get; set; }
        public string Text { get; set; }

        public decimal Price { get; set; }

        [CanBeNull]
        public IEnumerable<string> Files { get; set; }
    }
}