
using System.Collections.Generic;
namespace Zbang.Zbox.Infrastructure.Commands
{
    public interface ICommand
    {
    }

    public interface ICommandCache : ICommand
    {
        string CacheRegion { get; }
        List<string> CacheTags { get; set; }
    }

    public interface ICommandAsync : ICommand 
    { }


   
}
