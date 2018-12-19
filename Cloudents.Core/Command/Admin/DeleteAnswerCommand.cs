using System;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
{
    public class DeleteAnswerCommand : ICommand
    {

        public DeleteAnswerCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

    }
}
