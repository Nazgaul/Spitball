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
            await Task.Delay(TimeSpan.FromSeconds(Interval), cancellationToken);
        }

        protected abstract Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount, CancellationToken cancellationToken);

        protected const double MinInterval = 5;
        protected double Interval = MinInterval;
        protected double MaxInterval = TimeSpan.FromHours(1).TotalSeconds;
        protected abstract string GetPrefix();

        protected async Task SleepAndIncreaseIntervalAsync(CancellationToken cancellationToken)
        {

            await SleepAsync(cancellationToken);
            Interval = Math.Min(Interval * 2, MaxInterval);
        }

        protected async Task DoProcessAsync(CancellationToken cancellationToken, int index, int count)
        {
            var retVal = await UpdateAsync(index, count, cancellationToken);

            switch (retVal)
            {
                case TimeToSleep.Increase:
                    TraceLog.WriteInfo($"{GetPrefix()} going to sleep and increase {Interval}");
                    await SleepAndIncreaseIntervalAsync(cancellationToken);
                    break;
                case TimeToSleep.Min:
                    Interval = MinInterval;
                    TraceLog.WriteInfo($"{GetPrefix()} not going to sleep inteval change {Interval}");
                    break;
                case TimeToSleep.Same:
                    //TraceLog.WriteInfo($"{GetPrefix()} going to sleep inteval {Interval}");
                    await SleepAsync(cancellationToken);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
