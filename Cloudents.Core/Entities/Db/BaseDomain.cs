using System.Collections.Generic;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Entities.Db
{
    public abstract class BaseDomain : IEvents
    {
        protected BaseDomain()
        {
            Events = new List<IEvent>();
        }
        public IList<IEvent> Events { get; }
    }
}