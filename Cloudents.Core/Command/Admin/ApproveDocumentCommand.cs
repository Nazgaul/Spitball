using System.Collections.Generic;
using Cloudents.Application.Interfaces;

namespace Cloudents.Application.Command.Admin
{
    public class ApproveDocumentCommand : ICommand
    {
        public ApproveDocumentCommand(IEnumerable<long> id)
        {
            Id = id;
        }



        public IEnumerable<long> Id { get;private set; }
    }
}