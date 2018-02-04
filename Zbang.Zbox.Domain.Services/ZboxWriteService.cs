using System.Threading.Tasks;
using Zbang.Zbox.Domain.Commands;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Infrastructure.Cache;
using Zbang.Zbox.Infrastructure.CommandHandlers;
using Zbang.Zbox.Infrastructure.Data.NHibernateUnitOfWork;

namespace Zbang.Zbox.Domain.Services
{
    public class ZboxWriteService : IZboxWriteService, Autofac.IStartable
    {
        private readonly ICommandBus _mCommandBus;
        private readonly IWithCache _mCache;

        public ZboxWriteService(ICommandBus commandBus, IWithCache cache)
        {
            _mCommandBus = commandBus;
            _mCache = cache;
        }

        public void Start()
        {
            //using (UnitOfWork.Start())
            //{ }
        }

        


        public async Task DeleteItemAsync(DeleteItemCommand command)
        {
            using (var unitOfWork = UnitOfWork.Start())
            {
                var t1 = _mCommandBus.SendAsync(command);
                var t2 = _mCache.RemoveAsync(command);
                await Task.WhenAll(t1, t2).ConfigureAwait(true);
                unitOfWork.TransactionalFlush();
            }
        }

        public void RemoveOldConnections(RemoveOldConnectionCommand command)
        {
            using (UnitOfWork.Start())
            {
                _mCommandBus.Send(command);
                UnitOfWork.Current.TransactionalFlush();
            }
        }
    }
}
