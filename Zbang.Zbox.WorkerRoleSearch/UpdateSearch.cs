using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public abstract class UpdateSearch
    {
        public  readonly long[] JaredUniversityIdPilot = {
            164901L,
            166100L,
            167920L,
            168860L,
            171885L,
            171985L,
            172566L,
            173365L,
            173408L,
            173437L,
            173499L,
            173806L,
            173845L,
            173945L
        };
        protected async Task SleepAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(Interval), cancellationToken).ConfigureAwait(false);
        }

        protected abstract Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken);

        protected const double MinInterval = 5;
        protected double Interval = MinInterval;
        protected double MaxInterval = TimeSpan.FromHours(1).TotalSeconds;
        protected abstract string GetPrefix();

        protected async Task SleepAndIncreaseIntervalAsync(CancellationToken cancellationToken)
        {

            await SleepAsync(cancellationToken).ConfigureAwait(false);
            Interval = Math.Min(Interval * 2, MaxInterval);
        }

        protected async Task DoProcessAsync(CancellationToken cancellationToken, int index, int count)
        {
            var retVal = await UpdateAsync(index, count, cancellationToken).ConfigureAwait(false);

            switch (retVal)
            {
                case TimeToSleep.Increase:
                    //TraceLog.WriteInfo($"{GetPrefix()} going to sleep and increase {Interval}");
                    await SleepAndIncreaseIntervalAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case TimeToSleep.Min:
                    Interval = MinInterval;
                    //TraceLog.WriteInfo($"{GetPrefix()} not going to sleep interval change {Interval}");
                    break;
                case TimeToSleep.Same:
                    await SleepAsync(cancellationToken).ConfigureAwait(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
