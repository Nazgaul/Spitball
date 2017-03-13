using System;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public abstract class UpdateSearch
    {
        public const long JaredUniversityIdPilot = 173408;
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
                    TraceLog.WriteInfo($"{GetPrefix()} going to sleep and increase {Interval}");
                    await SleepAndIncreaseIntervalAsync(cancellationToken).ConfigureAwait(false);
                    break;
                case TimeToSleep.Min:
                    Interval = MinInterval;
                    TraceLog.WriteInfo($"{GetPrefix()} not going to sleep inteval change {Interval}");
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
