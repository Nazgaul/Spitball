using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Event;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.EventHandler
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Ioc inject")]
    public class RemoveQuestionCacheEventHandler : IEventHandler<MarkAsCorrectEvent>, 
        IEventHandler<QuestionCreatedEvent>, IEventHandler<QuestionDeletedEvent>, IEventHandler<AnswerCreatedEvent>, IEventHandler<AnswerDeletedEvent>
    {
        public const string CacheRegion = "Question";
        private readonly ICacheProvider _cacheProvider;

        public RemoveQuestionCacheEventHandler(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        private void RemoveFromCache()
        {
            _cacheProvider.DeleteRegion(CacheRegion);
        }

        public Task HandleAsync(MarkAsCorrectEvent eventMessage, CancellationToken token)
        {
            RemoveFromCache();
            return Task.CompletedTask;
        }

        public Task HandleAsync(QuestionCreatedEvent eventMessage, CancellationToken token)
        {
            RemoveFromCache();
            return Task.CompletedTask;
        }

        public Task HandleAsync(QuestionDeletedEvent eventMessage, CancellationToken token)
        {
            RemoveFromCache();
            return Task.CompletedTask;
        }

        public Task HandleAsync(AnswerCreatedEvent eventMessage, CancellationToken token)
        {
            RemoveFromCache();
            return Task.CompletedTask;
        }

        public Task HandleAsync(AnswerDeletedEvent eventMessage, CancellationToken token)
        {
            RemoveFromCache();
            return Task.CompletedTask;
        }
    }
}