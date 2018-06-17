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
       // Task UpVoteAsync(string userAddress, long questionId, Guid answerId, /*decimal price,*/ CancellationToken token);
        //Task<IEnumerable<string>> UpVoteListAsync(long questionId, Guid answerId, CancellationToken token);
        //Task SpreadFounds(long questionId, CancellationToken token);
    }
}
