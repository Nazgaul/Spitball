
namespace Zbang.Zbox.Infrastructure.Commands
{
    public interface ICommand
    {
    }

    public interface ICommandCache
    {
        string CacheRegion { get; }
    }

    public interface ICommandShare
    {
        bool NeedShare { get; }
    }

    public interface ICommandAsync : ICommand 
    { }


   
}
