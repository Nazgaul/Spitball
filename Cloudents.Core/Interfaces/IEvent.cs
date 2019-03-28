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
}