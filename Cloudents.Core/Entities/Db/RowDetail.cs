﻿using System;

namespace Cloudents.Core.Entities.Db
{
    public class RowDetail
    {
        public RowDetail()
        {
            CreationTime = UpdateTime = DateTime.UtcNow;
            CreatedUser = UpdatedUser = "new-sb";
        }

        public virtual DateTime CreationTime { get; private set; }
        public virtual DateTime UpdateTime { get; private set; }

        public virtual string CreatedUser { get; private set; }
        public virtual string UpdatedUser { get; private set; }

        //public void UpdateUserTime()
        //{
        //    UpdateTime = DateTime.UtcNow;
        //}
    }

    public class DomainTimeStamp
    {
        public DomainTimeStamp()
        {
            CreationTime = UpdateTime = DateTime.UtcNow;
        }

        public virtual DateTime CreationTime { get; private set; }
        public virtual DateTime UpdateTime { get; private set; }
    }
}
