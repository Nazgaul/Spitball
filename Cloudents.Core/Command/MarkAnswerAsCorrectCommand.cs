using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper handle that")]
    public class MarkAnswerAsCorrectCommand : ICommand
    {
        public Guid AnswerId { get; private set; }
        public long UserId { get; private set; }

        public long QuestionId { get; private set; }

        public string PrivateKey { get; private set; }
    }
}