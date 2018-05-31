using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainQAndAContract
    {
        Task SubmitQuestionAsync(string senderPk, long questionId, decimal price, CancellationToken token);
        Task SubmitAnswerAsync(string address, long questionId, Guid answerId, CancellationToken token);
        //take senderPk down and hardcode it in the function
        Task MarkAsCorrectAsync(string address, long questionId, CancellationToken token);
        //take senderPk down and hardcode it in the function
        Task UpVoteAsync(string address, long questionId, Guid answerId, decimal price, CancellationToken token);
        //take senderPk down and hardcode it in the function
        Task<string[]> UpVoteListAsync(long questionId, Guid answerId, CancellationToken token);
        //Task<string> UpVoteReword(long questionId, Guid answer, string upVoterAddr, CancellationToken token);
    }
}
