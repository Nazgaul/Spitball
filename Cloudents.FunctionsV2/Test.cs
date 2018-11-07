using Autofac;
using Cloudents.Core.Message.System;
using Cloudents.FunctionsV2.System;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class Test
    {
        [FunctionName("Test")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *", RunOnStartup = true)]TimerInfo timer,
            [Inject] ILifetimeScope lifetimeScope
            )
        {
            var command = new QuestionSearchMessage(true, null);


            var handlerType =
                typeof(ISystemOperation<>).MakeGenericType(command.GetType());

            dynamic operation = lifetimeScope.Resolve(handlerType);

            await operation.DoOperationAsync((dynamic)command, null, CancellationToken.None);
            //var operation = lifetimeScope.ResolveNamed<ISystemOperation>("ram");
        }


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
