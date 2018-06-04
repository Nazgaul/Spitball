using System.Collections.Concurrent;
using Autofac;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data
{
    //[UsedImplicitly]
    //public class UnitOfWorkAutofacFactory
    //{
    //    public UnitOfWorkAutofacFactory(IComponentContext componentContext)
    //    {
    //        _componentContext = componentContext;
    //        _instances = new ConcurrentDictionary<Core.Enum.Database, UnitOfWorkFactory>();
    //    }

    //    private readonly IComponentContext _componentContext;
    //    private readonly ConcurrentDictionary<Core.Enum.Database, UnitOfWorkFactory> _instances;

    //    public UnitOfWorkFactory GetInstance(Core.Enum.Database a)
    //    {
    //        return _instances.GetOrAdd(a, k =>
    //        {
    //            var f = _componentContext
    //                .Resolve<UnitOfWorkFactory.Factory>();
    //            return f.Invoke(k);
    //        });
    //    }
    //}
}