using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class RequestAccessLibraryNodeCommand : ICommandAsync
    {
        public RequestAccessLibraryNodeCommand(Guid departmentId, long userId)
        {
            UserId = userId;
            DepartmentId = departmentId;
        }

        public Guid DepartmentId { get;private set; }
        public long UserId { get; private set; }
    }
}
