using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Functions.Sync
{
    public interface IDbToSearchSync
    {
        SyncAzureQuery GetCurrentState();

        Task CreateIndex(CancellationToken token);


        Task<long> DoSync(SyncAzureQuery query, CancellationToken token);
        //Task<(IEnumerable<T> update, IEnumerable<long> delete, long version)>GetData<T>();

    }

    public class QuestionDbToSearchSync : IDbToSearchSync
    {
        private readonly ISearchServiceWrite<Question> _questionServiceWrite;
        private readonly IQueryBus _bus;

        public QuestionDbToSearchSync(ISearchServiceWrite<Question> questionServiceWrite, IQueryBus bus)
        {
            _questionServiceWrite = questionServiceWrite;
            _bus = bus;
        }

        public SyncAzureQuery GetCurrentState()
        {
            throw new System.NotImplementedException();
        }

        public Task CreateIndex(CancellationToken token)
        {
            return _questionServiceWrite.CreateOrUpdateAsync(token);
        }

        public async Task<long> DoSync(SyncAzureQuery query, CancellationToken token)
        {
            var (update, delete, version) =
                await _bus.QueryAsync(query, token);
            await _questionServiceWrite.UpdateDataAsync(update, delete.Select(s => s.ToString()), token);
            return version;
        }
    }
}