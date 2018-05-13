using Nethereum.Web3.Accounts;
using System.Numerics;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockchainProvider
    {

        //Task<BigInteger> GetBalanceAsync(string accountAddress);
        Task<Account> CreateAccount(); //return privete to user

        Task<BigInteger> GetTokenBalanceAsync(string senderPK);
        Task<string> TransferMoneyAsync(string senderPK, string toAddress, float amount);
        string GetPublicAddress(string privateKey); //Get Public from from privete

        
    }
}
