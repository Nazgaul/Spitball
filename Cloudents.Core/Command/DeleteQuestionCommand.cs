using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "automapper initialize it")]
    public class DeleteQuestionCommand : ICommand
    {
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "need for automapper")]
        public DeleteQuestionCommand()
        {
        }

        public long Id { get; set; }
        public long UserId { get; set; }

    }
}