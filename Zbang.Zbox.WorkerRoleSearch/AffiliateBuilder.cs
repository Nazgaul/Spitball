using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Interfaces;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public class AffiliateBuilder : ISchedulerProcess
    {
        private readonly IUpdateAffiliate _affiliate;

        public AffiliateBuilder(IUpdateAffiliate affiliate)
        {
            _affiliate = affiliate;
        }

        public async Task<bool> ExecuteAsync(int index, Func<int, TimeSpan, Task> progressAsync, CancellationToken token)
        {
            if (progressAsync == null) throw new ArgumentNullException(nameof(progressAsync));
            try
            {
                await _affiliate.ExecuteAsync(index, (i) => progressAsync(i, TimeSpan.FromMinutes(10)), token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                //_logger.Exception(ex, new Dictionary<string, string> { ["process"] = "AffiliateBuilder" });
                return false;
            }

            return true;
        }
    }
}
