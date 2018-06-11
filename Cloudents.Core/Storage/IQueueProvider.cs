using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Storage
{
    //public interface IQueueProvider
    //{
    //    //Task InsertMessageAsync<T>(T message, CancellationToken token);// where T : IQueueName;

    //    Task InsertEmailMessageAsync<T>(T obj, CancellationToken token) where T : QueueEmail;
    //    Task InsertBackgroundMessageAsync<T>(T obj, CancellationToken token) where T : QueueBackground;
    //}


    public interface IServiceBusProvider
    {
        //Task InsertMessageAsync<T>(T message, CancellationToken token);// where T : IQueueName;

        Task InsertMessageAsync(BaseEmail message, CancellationToken token);
        Task InsertMessageAsync(TalkJsUser message, CancellationToken token);
    }
}