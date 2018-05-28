using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainQAndAContract
    {
        Task<string> SubmitQuestionAsync(long question, decimal price, string senderAddress, CancellationToken token);
       
    }
}
