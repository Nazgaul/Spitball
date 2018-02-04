using System;
using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class LibraryNodeApproveAccessCommand : ICommandAsync
    {
        public LibraryNodeApproveAccessCommand(long userId, Guid departmentId, long approvedUserId)
        {
            ApprovedUserId = approvedUserId;
            DepartmentId = departmentId;
            UserId = userId;
        }

        public long UserId { get; private set; }
        public Guid DepartmentId { get; private set; }

        public long ApprovedUserId { get; private set; }
    }
}
