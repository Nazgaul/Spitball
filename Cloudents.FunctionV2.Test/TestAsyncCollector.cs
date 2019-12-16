using Microsoft.Azure.WebJobs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.Test
{
    public class TestAsyncCollector<T> : IAsyncCollector<T>
    {
        private readonly List<T> _list = new List<T>();

        public IEnumerable<T> Result => _list;

        public Task AddAsync(T item, CancellationToken cancellationToken = new CancellationToken())
        {
            _list.Add(item);
            return Task.CompletedTask;
        }

        public Task FlushAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return Task.CompletedTask;
            //Do nothing for now
        }
    }

   
}