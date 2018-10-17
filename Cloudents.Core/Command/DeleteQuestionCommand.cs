using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "automapper initialize it")]
    public class DeleteQuestionCommand : ICommand
    {
        public DeleteQuestionCommand(long id, long userId)
        {
            Id = id;
            UserId = userId;
        }

        public long Id { get; set; }
        public long UserId { get; set; }
    }
}