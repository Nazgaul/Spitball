using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Cloudents.Infrastructure.Write
{
    public abstract class UpdateAffiliate<T, TU> : IUpdateAffiliate
    {
        private readonly ILogger _logger;
        private readonly IDownloadFile _downloadFile;

        protected UpdateAffiliate(ILogger logger, IDownloadFile localStorage)
        {
            _logger = logger;
            _downloadFile = localStorage;
        }

        protected abstract string FileLocation { get; }
        protected abstract Uri Url { get; }

        protected abstract string Service { get; }

        protected abstract AuthenticationHeaderValue HttpHandler { get; }

        protected abstract IEnumerable<T> GetT(string location);
        protected abstract Task<TU> ParseTAsync(T obj, CancellationToken token);
        protected abstract Task UpdateSearchAsync(IEnumerable<TU> list, CancellationToken token);
        protected abstract Task DeleteOldItemsAsync(DateTime timeToDelete, CancellationToken token);

        public async Task ExecuteAsync(int index, Func<int, Task> progressAsync, CancellationToken token)
        {
            if (progressAsync == null) throw new ArgumentNullException(nameof(progressAsync));
            _logger.Info($"{Service} starting to work");
            var list = new List<Task<TU>>();
            var timeNow = DateTime.UtcNow;
            var (locationToSave, lastWriteTime) = await _downloadFile.DownloadFileAsync(Url, FileLocation, HttpHandler, token).ConfigureAwait(false);
            try
            {
                if (lastWriteTime < timeNow)
                {
                    if (index == 0)
                    {
                        //the data haven't change since last time
                        return;
                    }
                }
                else
                {
                    index = 0;
                }
                var i = 0;

                foreach (var job in GetT(locationToSave))
                {
                    i++;
                    if (i < index)
                    {
                        continue;
                    }

                    var obj = ParseTAsync(job, token);
                    list.Add(obj);
                    if (list.Count < 100) continue;
                    await Task.WhenAll(list).ConfigureAwait(false);
                    var t1 =  UpdateSearchAsync(list.Select(s => s.Result),
                        token); // m_JobSearchService.UpdateDataAsync(list, token);

                    var t2 = progressAsync.Invoke(i);
                    _logger.Info($"{Service} finish processing " + i);

                    await Task.WhenAll(t1, t2).ConfigureAwait(false);
                    token.ThrowIfCancellationRequested();
                    list.Clear();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.Info($"{Service} Task was Canceled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.Exception(ex, new Dictionary<string, string>()
                {
                    [nameof(Service)] = Service
                });
                throw;
            }
            if (list.Count > 0)
            {
                await Task.WhenAll(list).ConfigureAwait(false);
                await UpdateSearchAsync(list.Select(s => s.Result), token).ConfigureAwait(false);
            }
            _logger.Info($"{Service} Going To delete old items");
            await DeleteOldItemsAsync(lastWriteTime, token).ConfigureAwait(false);
            _logger.Info($"{Service} finish to work");
        }
    }
}