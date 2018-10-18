using Cloudents.Core.Interfaces;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.Command.Admin
{
    class DeleteAnswerCommand : ICommand
    {
            public DeleteAnswerCommand(Guid id)
            {
                Id = id;
            }

            public DeleteAnswerCommand()
            {
            }

            public Guid Id { get; set; }
           
    }
}
