using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Request;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainQAndAContract
    {
        Task SubmitAsync(BlockChainSubmitQuestion model, CancellationToken token);
        //Task SubmitAsync(BlockChainQnaSubmit model, CancellationToken token);
        Task SubmitAsync(BlockChainSubmitAnswer model, CancellationToken token);
        Task SubmitAsync(BlockChainMarkQuestionAsCorrect model, CancellationToken token);
        Task SubmitAsync(BlockChainUpVote model, CancellationToken token);
        //Task UpVoteAsync(string userAddress, long questionId, Guid answerId, CancellationToken token);
        //Task MarkAsCorrectAsync(string userAddress, string winnerAddress, long questionId, Guid answerId, CancellationToken token);
        //Task<IEnumerable<string>> UpVoteListAsync(long questionId, Guid answerId, CancellationToken token);
        //Task SpreadFounds(long questionId, CancellationToken token);
    }
}
