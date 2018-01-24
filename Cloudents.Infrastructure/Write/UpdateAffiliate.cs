using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        protected virtual HttpClientHandler HttpHandler()
        {
            return new HttpClientHandler();
        }

        protected abstract IEnumerable<T> GetT(string location);
        protected abstract Task<TU> ParseTAsync(T obj, CancellationToken token);
        protected abstract Task UpdateSearchAsync(IEnumerable<TU> list, CancellationToken token);
        protected abstract Task DeleteOldItemsAsync(CancellationToken token);

        public async Task ExecuteAsync(int index, Func<int, Task> progressAsync, CancellationToken token)
        {
            if (progressAsync == null) throw new ArgumentNullException(nameof(progressAsync));
            _logger.Info($"{Service} starting to work");

            var locationToSave = await _downloadFile.DownloadFileAsync(Url, FileLocation, index == 0, token).ConfigureAwait(false);

            var list = new List<Task<TU>>();
            var i = 0;
            try
            {
                foreach (var job in GetT(locationToSave))
                {
                    i++;
                    if (i < index)
                    {
                        continue;
                    }
                    var obj = ParseTAsync(job, token);
                    list.Add(obj);
                    if (list.Count <= 100) continue;
                    await Task.WhenAll(list).ConfigureAwait(false);
                    var t1 = UpdateSearchAsync(list.Select(s => s.Result), token); // m_JobSearchService.UpdateDataAsync(list, token);

                    var t2 = progressAsync.Invoke(i);
                    _logger.Info($"{Service} finish processing " + i);

                    await Task.WhenAll(t1, t2).ConfigureAwait(false);
                    token.ThrowIfCancellationRequested();
                    list.Clear();
                }
            }
            catch (Exception ex)
            {
                _logger.Exception(ex);
            }
            if (list.Count > 0)
            {
                await Task.WhenAll(list).ConfigureAwait(false);
                await UpdateSearchAsync(list.Select(s => s.Result), token).ConfigureAwait(false);
            }
            _logger.Info($"{Service} Going To delete old items");
            await DeleteOldItemsAsync(token).ConfigureAwait(false);
            _logger.Info($"{Service} finish to work");
        }
    }
}