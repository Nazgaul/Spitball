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
}