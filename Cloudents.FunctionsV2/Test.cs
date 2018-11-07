using System.Threading;
using Cloudents.Infrastructure.Database.Query;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using Cloudents.Core.Command.Admin;
using Cloudents.Core.Interfaces;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class Test
    {
        //[FunctionName("Test"), Disable]
        //public static async Task Run(
        //    [TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo timer,
        //   [Blob("spitball-files/files/1/text.txt")] string text, IDictionary<string, string> metadata
        //    )
        //{


        //}


        //[FunctionName("RemoveDuplicatePendingQuestion"),Disb]
        //public static async Task Run(
        //    [TimerTrigger("0 */20 * * * *", RunOnStartup = true)]TimerInfo timer,
        //    [Inject] ReadonlyStatelessSession session,
        //    [Inject] ICommandBus bus,
        //    CancellationToken token
        //)
        //{
        //    var query = session.Session.CreateSQLQuery(@"WITH CTE AS(
        //    SELECT id,
        //        RN = ROW_NUMBER()OVER(PARTITION BY Text ORDER BY Text)
        //    from sb.question where State = 'pending'
        //            )
        //        select id  FROM CTE WHERE RN > 1");
        //    var ids = await query.ListAsync<long>(token);
        //    foreach (var id in ids)
        //    {
        //        var command = new DeleteQuestionCommand(id);
        //        await bus.DispatchAsync(command, token);


        //    }



        //}
    }
}
