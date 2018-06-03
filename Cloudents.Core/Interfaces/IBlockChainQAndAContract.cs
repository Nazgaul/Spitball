using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainQAndAContract
    {
        Task SubmitQuestionAsync(long questionId, decimal price, string userAddress, CancellationToken token);
        Task SubmitAnswerAsync(long questionId, Guid answerId, CancellationToken token);
        Task MarkAsCorrectAsync(string userAddress, string winnerAddress, long question, Guid answerId, CancellationToken token);
        //take senderPk down and hardcode it in the function
        Task UpVoteAsync(string userAddress, long question, Guid answerId, CancellationToken token);
        //take senderPk down and hardcode it in the function
        Task<string[]> UpVoteListAsync(long questionId, Guid answerId, CancellationToken token);
        //Task<string> UpVoteReword(long questionId, Guid answer, string upVoterAddr, CancellationToken token);
    }
}
