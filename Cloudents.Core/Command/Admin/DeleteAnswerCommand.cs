using Cloudents.Core.Interfaces;
using System;

namespace Cloudents.Core.Command.Admin
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
