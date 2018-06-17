using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainErc20Service
    {
        //(string privateKey, string publicAddress) CreateAccount();
        Task<decimal> GetBalanceAsync(string senderAddress, CancellationToken token);
        Task<string> TransferMoneyAsync(string senderPk, string toAddress, float amount, CancellationToken token);
        Task SetInitialBalanceAsync(string address, CancellationToken token);
        //string GetPublicAddress(string privateKey);
        Task<string> CreateNewTokens(string toAddress, int amount, CancellationToken token);

        string GetAddress(string privateKey);

        (string privateKey, string publicAddress) CreateAccount();

    }
}
