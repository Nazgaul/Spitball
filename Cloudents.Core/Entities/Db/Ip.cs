using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Core.Entities.Db
{
    public class Ip
    {
        public virtual int Id { get;protected set; }
        public virtual uint From { get; protected set; }
        public virtual uint To { get; protected set; }
        public virtual string CountryCode { get; protected set; }
        public virtual string CountryName { get; protected set; }
    }
}
