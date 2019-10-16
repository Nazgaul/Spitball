using System;

namespace Cloudents.Core.Interfaces
{
    public interface IEvent
    {

    }

    public interface IBinarySerializer
    {
        byte[] Serialize(object o);
        T Deserialize<T>(byte[] sr);
    }

    public interface IJsonSerializer
    {
        string Serialize(object o);

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