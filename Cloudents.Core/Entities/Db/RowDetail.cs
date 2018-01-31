using System;

namespace Cloudents.Core.Entities.Db
{
    public class RowDetail
    {
        public RowDetail()
        {
            CreationTime = UpdateTime = DateTime.UtcNow;
            CreatedUser = UpdatedUser = "sys";
        }

        public DateTime CreationTime { get; private set; }
        public DateTime UpdateTime { get; private set; }

        public string CreatedUser { get; private set; }
        public string UpdatedUser { get; private set; }

        public void UpdateUserTime()
        {
            UpdateTime = DateTime.UtcNow;
        }
    }
}
