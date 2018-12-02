using System.Collections.Generic;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Command.Admin
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