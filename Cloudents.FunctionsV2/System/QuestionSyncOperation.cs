using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.System
{
    public class QuestionSyncOperation : ISystemOperation<QuestionSearchMessage>
    {
        private readonly ISearchServiceWrite<Question> _questionServiceWrite;

        public QuestionSyncOperation(ISearchServiceWrite<Question> questionServiceWrite)
        {
            _questionServiceWrite = questionServiceWrite;
        }

        public Task DoOperationAsync(QuestionSearchMessage msg, IBinder binder, CancellationToken token)
        {
            if (msg.ShouldInsert)
            {
                return _questionServiceWrite.UpdateDataAsync(new[] { msg.Question }, token);
            }
            return _questionServiceWrite.DeleteDataAsync(new[] { msg.Question.Id }, token);
        }
    }
}