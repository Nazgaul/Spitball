using Nethereum.Web3.Accounts;
using System.Numerics;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainProvider
    {

        //Task<BigInteger> GetBalanceAsync(string accountAddress);
        Account CreateAccount(); //return private to user

        Task<BigInteger> GetTokenBalanceAsync(string senderPk);
        Task<string> TransferMoneyAsync(string senderPk, string toAddress, float amount);
        string GetPublicAddress(string privateKey); //Get Public from from private
        Task<bool> SetInitialBalance(string address);



    }
}
