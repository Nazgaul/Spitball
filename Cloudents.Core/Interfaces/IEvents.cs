using System.Collections.Generic;

namespace Cloudents.Core.Interfaces
{
    public interface IEvents
    {
        IList<IEvent> Events { get; }
    }
}