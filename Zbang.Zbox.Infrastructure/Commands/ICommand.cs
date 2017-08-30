
using Zbang.Zbox.Infrastructure.Cache;

namespace Zbang.Zbox.Infrastructure.Commands
{
    public interface ICommand
    {
    }

    public interface ICommandCache
    {
        CacheRegions CacheRegion { get; }
    }

    //public interface ICommandShare
    //{
    //    bool NeedShare { get; }
    //}

    public interface ICommandAsync : ICommand 
    { }
   
}
