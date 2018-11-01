using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Message.System;
using Microsoft.Azure.WebJobs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.FunctionsV2.System
{
    public class QuestionSyncOperation : ISystemOperation
    {
        private readonly ISearchServiceWrite<Question> _questionServiceWrite;


        public QuestionSyncOperation(ISearchServiceWrite<Question> questionServiceWrite)
        {
            _questionServiceWrite = questionServiceWrite;
        }

        public Task DoOperationAsync(BaseSystemMessage msg, IBinder binder, CancellationToken token)
        {
            var d = msg.GetData();
            if (!(d is QuestionSearchMessage p))
            {
                throw new ArgumentException("QuestionSyncOperation is not QuestionSearchMessage");
            }

            if (p.ShouldInsert)
            {
                return _questionServiceWrite.UpdateDataAsync(new[] { p.Question }, token);
            }

            return _questionServiceWrite.DeleteDataAsync(new[] { p.Question.Id }, token);
        }
    }
}