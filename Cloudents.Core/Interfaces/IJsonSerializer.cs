using System;
using Cloudents.Core.Message.System;

namespace Cloudents.Core.Interfaces
{
    public interface IJsonSerializer
    {
        string Serialize(object o);
        string Serialize(ISystemQueueMessage obj);

        T Deserialize<T>(string sr);

        T Deserialize<T>(string sr, JsonConverterTypes types);
    }
    [Flags]
    public enum JsonConverterTypes
    {
        None = 0x0,
        TimeSpan = 0x1
    }
}