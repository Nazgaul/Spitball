using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Trace;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public abstract class UpdateSearch
    {
        protected async Task SleepAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(Interval), cancellationToken);
        }

        protected abstract Task<TimeToSleep> UpdateAsync(int instanceId, int instanceCount);

        protected const int MinInterval = 10;
        protected int Interval = MinInterval;
        protected abstract string GetPrefix();

        protected async Task SleepAndIncreaseIntervalAsync(CancellationToken cancellationToken)
        {
            
            await SleepAsync(cancellationToken);
            Interval = Interval * 2;
        }

        protected async Task DoProcessAsync(CancellationToken cancellationToken, int index, int count)
        {
            var retVal = await UpdateAsync(index, count);
            
        
        
        switch (

        retVal)
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
                    TraceLog.WriteInfo($"{GetPrefix()} going to sleep inteval {Interval}");
                    await SleepAsync(cancellationToken);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
