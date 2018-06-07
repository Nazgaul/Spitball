using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainCrowdSaleService
    {
        Task<string> BuyTokensAsync(string senderPrivateKey, int amount, CancellationToken token);
        Task Withdrawal(CancellationToken token);

    }
}
