using System.Collections.Generic;

namespace Cloudents.Domain.Entities
{
    public abstract class DomainObject 
    {
        protected DomainObject()
        {
            //   Events = new List<IEvent>();
        }
        //  public virtual IList<IEvent> Events { get; }
    }
}