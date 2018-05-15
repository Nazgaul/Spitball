namespace Cloudents.Core.Interfaces
{
    public interface IMapper
    {
        T Map<T>(object source);
    }
}