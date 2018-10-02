using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainErc20Service
    {
       
        Task<decimal> GetBalanceAsync(string senderAddress, CancellationToken token);
        Task<string> TransferMoneyAsync(string senderPk, string toAddress, float amount, CancellationToken token);
        Task SetInitialBalanceAsync(string address, CancellationToken token);
        //string GetPublicAddress(string privateKey);
        Task<string> CreateNewTokens(string toAddress, int amount, CancellationToken token);

        string GetAddress(string privateKey);

        (string privateKey, string publicAddress) CreateAccount();

        Task<string> Approve(string spender, int amount, CancellationToken token);
       // Task<string> ApprovePreSigned(string sig, string spender, int amount, int fee, int nonce);
    }
}
