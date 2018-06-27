using System;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Db
{
    public class Audit
    {
        public Audit(ICommand command)
        {
            Command = command;
            DateTime = DateTime.UtcNow;
        }

        public Audit()
        {

        }

        public virtual Guid Id { get; set; }

        public virtual ICommand Command { get; set; }

        public virtual DateTime DateTime { get; set; }
    }
}