using Nethereum.Web3.Accounts;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IBlockChainProvider
    {
        //Task<BigInteger> GetBalanceAsync(string accountAddress);
        Account CreateAccount(); //return private to user
        string GetPublicAddress(string privateKey); //Get Public from from private

        //Task<string> BuyTokens(string senderPK,  int amount, CancellationToken token);


    }
}
