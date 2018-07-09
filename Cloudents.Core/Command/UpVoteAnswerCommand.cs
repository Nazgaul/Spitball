using System;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global" , Justification = "Automapper")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Automapper")]
    public class UpVoteAnswerCommand : ICommand
    {
        public Guid Id { get; private set; }

        public long UserId { get; set; }
    }
}