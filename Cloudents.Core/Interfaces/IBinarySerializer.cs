namespace Cloudents.Core.Interfaces
{
    public interface IBinarySerializer
    {
        byte[] Serialize<T>(T value);
        T DeSerialize<T>(byte[] input);
    }
}