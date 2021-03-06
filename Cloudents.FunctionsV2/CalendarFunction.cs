using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    [SuppressMessage("ReSharper", "AsyncConverter.AsyncAwaitMayBeElidedHighlighting")]
    public static class CalendarFunction
    {
        [FunctionName("CalendarFunction")]
        public static async Task RunAsync(
            [TimerTrigger("0 */30 * * * *")]TimerInfo myTimer,
            [Inject] ICalendarService calendarService,
            CancellationToken token)
        {
            await calendarService.DeleteDeclinedEventCalendarAsync(token);
        }
    }
}
