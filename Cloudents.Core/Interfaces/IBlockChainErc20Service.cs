using Nethereum.Web3.Accounts;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainErc20Service
    {
        Account CreateAccount(); //return private to user
        Task<decimal> GetBalanceAsync(string senderAddress, CancellationToken token);
        Task<string> TransferMoneyAsync(string senderPk, string toAddress, float amount, CancellationToken token);
        
        Task<bool> SetInitialBalanceAsync(string address, CancellationToken token);
    }
}
