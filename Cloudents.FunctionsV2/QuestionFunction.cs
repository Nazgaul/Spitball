using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Command;
using Cloudents.Command.Command.Admin;
using Cloudents.Query;
using Cloudents.Query.Questions;
using Microsoft.Azure.WebJobs;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2
{
    public static class QuestionFunction
    {
        [FunctionName("QuestionPopulate")]
        public static async Task QuestionPopulateAsync([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer,
            [Inject] ICommandBus commandBus,
            [Inject] IQueryBus queryBus,
            CancellationToken token)
        {
            var questions = await queryBus.QueryAsync(new FictivePendingQuestionEmptyQuery(), token);
            if (questions.Count > 0)
            {
                var command = new ApproveQuestionCommand(questions.Select(s => s.Id));
                await commandBus.DispatchAsync(command, token);
            }
        }
    }
}
