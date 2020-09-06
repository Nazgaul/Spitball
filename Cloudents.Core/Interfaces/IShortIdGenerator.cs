namespace Cloudents.Core.Interfaces
{
    public interface IShortIdGenerator
    {
        string GenerateShortId(int length);
    }
}