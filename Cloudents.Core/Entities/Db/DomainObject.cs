using System.Collections.Generic;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Db
{
    public abstract class DomainObject //: IEvents
    {
        protected DomainObject()
        {
           // Events = new List<IEvent>();
        }
        //public virtual IList<IEvent> Events { get; }
    }
}