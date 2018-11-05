using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.EventHandler
{
    public class CacheRemoveEventHandler : IEventHandler<UserChangeCoursesEvent>
    {
        private readonly ICacheProvider _cacheProvider;

        public CacheRemoveEventHandler(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public Task HandleAsync(UserChangeCoursesEvent eventMessage, CancellationToken token)
        {
            _cacheProvider.DeleteKey("user-courses",eventMessage.User.Id.ToString());
            return Task.CompletedTask;
        }
    }
}