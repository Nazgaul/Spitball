namespace Cloudents.Core.Interfaces
{
    public interface IStreamSerializer
    {
        byte[] Serialize<T>(T value);
        T DeSerialize<T>(byte[] bytes);
    }
}