namespace Cloudents.Core.Interfaces
{
    public interface IKeyGenerator
    {
        string GenerateKey(object sourceObject);
    }
}