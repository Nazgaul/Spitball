using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainErc20Service
    {
       
        Task<decimal> GetBalanceAsync(string senderAddress, CancellationToken token);
        Task<string> TransferMoneyAsync(string senderPk, string toAddress, float amount, CancellationToken token);
        Task SetInitialBalanceAsync(string address, CancellationToken token);
        Task<string> MintNewTokens(string toAddress, int amount, CancellationToken token);

        string GetAddress(string privateKey);

        (string privateKey, string publicAddress) CreateAccount();

        Task<string> ApproveAsync(string spender, int amount, CancellationToken token);
        Task<string> IncreaseApprovalAsync(string spender, int amount, CancellationToken token);
        //Task<string> TransferPreSignedAsync(string fromPK, string to, int amount, int fee, CancellationToken token);
        Task<string> TransferPreSignedAsync(string delegatePK, string fromPK, string to, int amount, int fee, CancellationToken token);
        Task<string> ApprovePreSignedAsync(string fromPK, string sender, int amount, int fee, CancellationToken token);
        Task<string> IncreaseApprovalPreSignedAsync(string fromPK, string sender, int amount, int fee, CancellationToken token);
        Task<decimal> GetAllowanceAsync(string ownerAddress, string spenderAddress, CancellationToken token);

        Task<string> WhitelistUserForTransfers(string userAddress, CancellationToken token);
    }
}
